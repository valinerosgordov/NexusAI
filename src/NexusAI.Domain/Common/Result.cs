namespace NexusAI.Domain.Common;

public readonly record struct Result<T>
{
    private readonly T? _value;
    private readonly string? _error;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot access Value on failed Result: {_error}");

    public string Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("Cannot access Error on successful Result");

    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        _error = null;
    }

    private Result(string error)
    {
        IsSuccess = false;
        _value = default;
        _error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
        => IsSuccess ? onSuccess(Value) : onFailure(Error);

    public Result<TNext> Map<TNext>(Func<T, TNext> mapper)
        => IsSuccess ? Result<TNext>.Success(mapper(Value)) : Result<TNext>.Failure(Error);

    public async Task<Result<TNext>> MapAsync<TNext>(Func<T, Task<TNext>> mapper)
        => IsSuccess ? Result<TNext>.Success(await mapper(Value)) : Result<TNext>.Failure(Error);

    public Result<TNext> Bind<TNext>(Func<T, Result<TNext>> binder)
        => IsSuccess ? binder(Value) : Result<TNext>.Failure(Error);

    public async Task<Result<TNext>> BindAsync<TNext>(Func<T, Task<Result<TNext>>> binder)
        => IsSuccess ? await binder(Value) : Result<TNext>.Failure(Error);
}

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
}
