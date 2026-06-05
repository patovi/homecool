namespace HomeCool.Api.DTOs.Users;

public record UserDto(int Id, string Name, string Role, bool IsActive, DateTime CreatedAt);

public record CreateUserRequest(string Name, string Password, string Role = "User");

public record UpdateUserRequest(string? Name, string? Password, string? Role, bool? IsActive);
