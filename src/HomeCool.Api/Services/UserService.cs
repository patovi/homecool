using HomeCool.Api.Data;
using HomeCool.Api.DTOs.Users;
using HomeCool.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeCool.Api.Services;

public class UserService(AppDbContext db)
{
    public async Task<List<UserDto>> GetAllAsync() =>
        await db.Users
            .OrderBy(u => u.Name)
            .Select(u => new UserDto(u.Id, u.Name, u.Role, u.IsActive, u.CreatedAt))
            .ToListAsync();

    public async Task<UserDto?> GetByIdAsync(int id) =>
        await db.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDto(u.Id, u.Name, u.Role, u.IsActive, u.CreatedAt))
            .FirstOrDefaultAsync();

    public async Task<(UserDto? User, string? Error)> CreateAsync(CreateUserRequest request)
    {
        if (await db.Users.AnyAsync(u => u.Name == request.Name))
            return (null, "Benutzername bereits vergeben.");

        var user = new User
        {
            Name = request.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role is "Admin" or "User" ? request.Role : "User"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return (new UserDto(user.Id, user.Name, user.Role, user.IsActive, user.CreatedAt), null);
    }

    public async Task<(UserDto? User, string? Error)> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return (null, "Benutzer nicht gefunden.");

        if (request.Name is not null)
        {
            if (await db.Users.AnyAsync(u => u.Name == request.Name && u.Id != id))
                return (null, "Benutzername bereits vergeben.");
            user.Name = request.Name;
        }

        if (request.Password is not null)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        if (request.Role is "Admin" or "User")
            user.Role = request.Role;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        await db.SaveChangesAsync();
        return (new UserDto(user.Id, user.Name, user.Role, user.IsActive, user.CreatedAt), null);
    }
}
