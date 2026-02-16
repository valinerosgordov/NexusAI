namespace NexusAI.Domain.Common;

public static class ResultExtensions
{
    public static Result<T> ToResult<T>(this T? value, string errorMessage)
        => value is not null ? Result<T>.Success(value) : Result<T>.Failure(errorMessage);

    public static async Task<Result<TOut>> SelectAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<TOut>> selector)
        => result.IsSuccess
            ? Result<TOut>.Success(await selector(result.Value).ConfigureAwait(false))
            : Result<TOut>.Failure(result.Error);

    public static Result<TOut> Select<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> selector)
        => result.IsSuccess
            ? Result<TOut>.Success(selector(result.Value))
            : Result<TOut>.Failure(result.Error);

    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);
        return result;
    }

    public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
    {
        if (result.IsFailure)
            action(result.Error);
        return result;
    }

    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        string errorMessage)
        => result.IsSuccess && !predicate(result.Value)
            ? Result<T>.Failure(errorMessage)
            : result;
}
