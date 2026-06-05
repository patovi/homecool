namespace HomeCool.Api.DTOs.Auth;

public record LoginResponse(string Token, string Name, string Role, int UserId);
