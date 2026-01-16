using iCreditoApi.Shared.Domain.Primitives;
using iCreditoApi.Modules.Auth.Domain.Events;

namespace iCreditoApi.Modules.Auth.Domain.Entities;

/// <summary>
/// Entidad User - Agregado raíz del módulo de autenticación
/// </summary>
public class User : AggregateRoot
{
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private User() : base() { }

    private User(
        Guid id,
        string username,
        string email,
        string passwordHash,
        string firstName,
        string lastName) : base(id)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// Factory method para crear un nuevo usuario
    /// </summary>
    public static User Create(
        string username,
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        var user = new User(
            Guid.NewGuid(),
            username.ToLowerInvariant().Trim(),
            email.ToLowerInvariant().Trim(),
            passwordHash,
            firstName.Trim(),
            lastName.Trim());

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.Username));

        return user;
    }

    /// <summary>
    /// Verifica si la contraseña es correcta
    /// </summary>
    public bool VerifyPassword(string password, Func<string, string, bool> verifier)
    {
        return verifier(password, PasswordHash);
    }

    /// <summary>
    /// Registra el inicio de sesión
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        AddDomainEvent(new UserLoggedInEvent(Id));
    }

    /// <summary>
    /// Actualiza el perfil del usuario
    /// </summary>
    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    /// <summary>
    /// Cambia la contraseña
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    /// <summary>
    /// Desactiva el usuario
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Reactiva el usuario
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }
}
