namespace iCreditoApi.Shared.Application.Result;

/// <summary>
/// Representa el resultado de una operación que puede fallar
/// Implementa el patrón Result para evitar excepciones en el flujo normal
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Un resultado exitoso no puede tener error");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Un resultado fallido debe tener un error");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

/// <summary>
/// Resultado genérico con valor
/// </summary>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("No se puede acceder al valor de un resultado fallido");

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public static implicit operator Result<TValue>(TValue? value) => Create(value);

    public Result<TResult> Map<TResult>(Func<TValue, TResult> mapper)
    {
        return IsSuccess
            ? Result.Success(mapper(Value))
            : Result.Failure<TResult>(Error);
    }

    public async Task<Result<TResult>> MapAsync<TResult>(Func<TValue, Task<TResult>> mapper)
    {
        return IsSuccess
            ? Result.Success(await mapper(Value))
            : Result.Failure<TResult>(Error);
    }

    public Result<TValue> Tap(Action<TValue> action)
    {
        if (IsSuccess)
            action(Value);
        return this;
    }

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}

/// <summary>
/// Extensiones para trabajar con Result
/// </summary>
public static class ResultExtensions
{
    public static Result<T> ToResult<T>(this T? value, Error error) where T : class
    {
        return value is not null ? Result.Success(value) : Result.Failure<T>(error);
    }

    public static Result<T> ToResult<T>(this T? value, Error error) where T : struct
    {
        return value.HasValue ? Result.Success(value.Value) : Result.Failure<T>(error);
    }

    public static async Task<Result> Bind(this Result result, Func<Task<Result>> func)
    {
        return result.IsFailure ? result : await func();
    }

    public static async Task<Result<TResult>> Bind<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, Task<Result<TResult>>> func)
    {
        return result.IsFailure
            ? Result.Failure<TResult>(result.Error)
            : await func(result.Value);
    }
}
