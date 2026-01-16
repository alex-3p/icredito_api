using iCreditoApi.Modules.Auth.Application.DTOs;
using iCreditoApi.Modules.Auth.Application.Errors;
using iCreditoApi.Modules.Auth.Application.Ports;
using iCreditoApi.Modules.Auth.Domain.Entities;
using iCreditoApi.Modules.Auth.Domain.Repositories;
using iCreditoApi.Shared.Application.Interfaces;
using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.Modules.Auth.Application.Services;

/// <summary>
/// Servicio de aplicación para autenticación
/// Implementa los casos de uso del módulo Auth
/// </summary>
public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    public async Task<Result<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken ct = default)
    {
        // Validar que el username no exista
        if (await _userRepository.ExistsByUsernameAsync(request.Username, ct))
            return Result.Failure<AuthResponse>(AuthErrors.UsernameAlreadyExists);

        // Validar que el email no exista
        if (await _userRepository.ExistsByEmailAsync(request.Email, ct))
            return Result.Failure<AuthResponse>(AuthErrors.EmailAlreadyExists);

        // Hashear la contraseña
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Crear el usuario
        var user = User.Create(
            request.Username,
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName);

        // Persistir
        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Generar token
        var token = _tokenService.GenerateAccessToken(user);

        return Result.Success(new AuthResponse(
            user.Id,
            user.Username,
            user.Email,
            token,
            DateTime.UtcNow.AddHours(1)));
    }

    /// <summary>
    /// Inicia sesión de un usuario
    /// </summary>
    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken ct = default)
    {
        // Buscar usuario por username
        var user = await _userRepository.GetByUsernameAsync(request.Username, ct);

        if (user is null)
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        // Verificar que esté activo
        if (!user.IsActive)
            return Result.Failure<AuthResponse>(AuthErrors.UserInactive);

        // Verificar contraseña
        if (!user.VerifyPassword(request.Password, _passwordHasher.Verify))
            return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

        // Registrar login
        user.RecordLogin();
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        // Generar token
        var token = _tokenService.GenerateAccessToken(user);

        return Result.Success(new AuthResponse(
            user.Id,
            user.Username,
            user.Email,
            token,
            DateTime.UtcNow.AddHours(1)));
    }

    /// <summary>
    /// Obtiene el perfil del usuario actual
    /// </summary>
    public async Task<Result<UserDto>> GetProfileAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            return Result.Failure<UserDto>(AuthErrors.UserNotFound);

        return Result.Success(MapToDto(user));
    }

    /// <summary>
    /// Actualiza el perfil del usuario
    /// </summary>
    public async Task<Result<UserDto>> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            return Result.Failure<UserDto>(AuthErrors.UserNotFound);

        user.UpdateProfile(request.FirstName, request.LastName);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToDto(user));
    }

    private static UserDto MapToDto(User user) => new(
        user.Id,
        user.Username,
        user.Email,
        user.FirstName,
        user.LastName,
        user.FullName,
        user.CreatedAt,
        user.LastLoginAt,
        user.IsActive);
}
