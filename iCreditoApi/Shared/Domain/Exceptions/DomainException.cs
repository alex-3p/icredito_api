namespace iCreditoApi.Shared.Domain.Exceptions;

/// <summary>
/// Excepción base para errores de dominio
/// </summary>
public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string message) : base(message)
    {
        Code = "DOMAIN_ERROR";
    }

    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
        Code = "DOMAIN_ERROR";
    }
}

/// <summary>
/// Excepción para errores de validación
/// </summary>
public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("VALIDATION_ERROR", "Se encontraron uno o más errores de validación")
    {
        Errors = errors;
    }

    public ValidationException(string field, string message)
        : base("VALIDATION_ERROR", message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }
}

/// <summary>
/// Excepción cuando no se encuentra una entidad
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base("NOT_FOUND", $"La entidad '{entityName}' con identificador '{key}' no fue encontrada")
    {
    }
}

/// <summary>
/// Excepción para operaciones no autorizadas
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "No tiene permisos para realizar esta operación")
        : base("UNAUTHORIZED", message)
    {
    }
}
