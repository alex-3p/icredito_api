namespace iCreditoApi.Shared.Application.Result;

/// <summary>
/// Representa un error en la aplicación
/// </summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "El valor proporcionado es nulo");

    public static Error NotFound(string entityName, object key) =>
        new("Error.NotFound", $"La entidad '{entityName}' con identificador '{key}' no fue encontrada");

    public static Error Validation(string message) =>
        new("Error.Validation", message);

    public static Error Unauthorized(string message = "No autorizado") =>
        new("Error.Unauthorized", message);

    public static Error Conflict(string message) =>
        new("Error.Conflict", message);

    public static Error Failure(string message) =>
        new("Error.Failure", message);

    public override string ToString() => $"[{Code}] {Message}";
}
