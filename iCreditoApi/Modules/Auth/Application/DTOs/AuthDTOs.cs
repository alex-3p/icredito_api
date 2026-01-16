namespace iCreditoApi.Modules.Auth.Application.DTOs;

// Request DTOs
public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName);

public record LoginRequest(
    string Username,
    string Password);

public record UpdateProfileRequest(
    string FirstName,
    string LastName);

// Response DTOs
public record AuthResponse(
    Guid UserId,
    string Username,
    string Email,
    string Token,
    DateTime ExpiresAt);

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    bool IsActive);
