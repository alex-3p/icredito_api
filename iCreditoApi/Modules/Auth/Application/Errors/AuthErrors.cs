using iCreditoApi.Shared.Application.Result;

namespace iCreditoApi.Modules.Auth.Application.Errors;

/// <summary>
/// Errores específicos del módulo de autenticación
/// </summary>
public static class AuthErrors
{
    public static readonly Error UsernameAlreadyExists =
        new("Auth.UsernameExists", "El nombre de usuario ya está en uso");

    public static readonly Error EmailAlreadyExists =
        new("Auth.EmailExists", "El correo electrónico ya está registrado");

    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Usuario o contraseña incorrectos");

    public static readonly Error UserNotFound =
        new("Auth.UserNotFound", "Usuario no encontrado");

    public static readonly Error UserInactive =
        new("Auth.UserInactive", "La cuenta de usuario está desactivada");

    public static readonly Error InvalidPassword =
        new("Auth.InvalidPassword", "La contraseña no cumple con los requisitos mínimos");

    public static readonly Error InvalidEmail =
        new("Auth.InvalidEmail", "El formato del correo electrónico no es válido");
}
