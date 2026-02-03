namespace NexusAI.Domain.Common;

/// <summary>
/// Railway Oriented Programming extensions for Result<T>.
/// Enables fluent chaining and functional composition of operations.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Executes an action if result is successful, returns original result.
    /// Useful for side effects (logging, UI updates) in success path.
    /// </summary>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Executes an action if result is failure, returns original result.
    /// Useful for side effects (logging, error handling) in failure path.
    /// </summary>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
    {
        if (result.IsFailure)
        {
            action(result.Error);
        }
        return result;
    }

    /// <summary>
    /// Async version of OnSuccess.
    /// </summary>
    public static async Task<Result<T>> OnSuccessAsync<T>(this Result<T> result, Func<T, Task> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Async version of OnFailure.
    /// </summary>
    public static async Task<Result<T>> OnFailureAsync<T>(this Result<T> result, Func<string, Task> action)
    {
        if (result.IsFailure)
        {
            await action(result.Error);
        }
        return result;
    }

    /// <summary>
    /// Transforms success value or provides default on failure.
    /// Unwraps Result to direct value with fallback.
    /// </summary>
    public static T ValueOr<T>(this Result<T> result, T defaultValue)
        => result.IsSuccess ? result.Value : defaultValue;

    /// <summary>
    /// Transforms success value using factory or provides default on failure.
    /// </summary>
    public static T ValueOr<T>(this Result<T> result, Func<string, T> errorFactory)
        => result.IsSuccess ? result.Value : errorFactory(result.Error);

    /// <summary>
    /// Ensures result meets a condition, converts to failure if not.
    /// Useful for validation chains.
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string errorMessage)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value)
            ? result
            : Result.Failure<T>(errorMessage);
    }

    /// <summary>
    /// Async version of Ensure.
    /// </summary>
    public static async Task<Result<T>> EnsureAsync<T>(
        this Result<T> result, 
        Func<T, Task<bool>> predicate, 
        string errorMessage)
    {
        if (result.IsFailure)
            return result;

        return await predicate(result.Value)
            ? result
            : Result.Failure<T>(errorMessage);
    }

    /// <summary>
    /// Combines two results - both must succeed for success.
    /// Useful for operations that depend on multiple results.
    /// </summary>
    public static Result<(T1, T2)> Combine<T1, T2>(
        this Result<T1> result1, 
        Result<T2> result2)
    {
        if (result1.IsFailure)
            return Result.Failure<(T1, T2)>(result1.Error);

        if (result2.IsFailure)
            return Result.Failure<(T1, T2)>(result2.Error);

        return Result.Success((result1.Value, result2.Value));
    }

    /// <summary>
    /// Tries to execute a function, wrapping exceptions in Result.
    /// Converts exception-based code to Railway Oriented style.
    /// </summary>
    public static Result<T> Try<T>(Func<T> func)
    {
        try
        {
            return Result.Success(func());
        }
        catch (Exception ex)
        {
            return Result.Failure<T>(ex.Message);
        }
    }

    /// <summary>
    /// Async version of Try.
    /// </summary>
    public static async Task<Result<T>> TryAsync<T>(Func<Task<T>> func)
    {
        try
        {
            var value = await func();
            return Result.Success(value);
        }
        catch (Exception ex)
        {
            return Result.Failure<T>(ex.Message);
        }
    }

    /// <summary>
    /// Flattens nested Result<Result<T>> to Result<T>.
    /// </summary>
    public static Result<T> Flatten<T>(this Result<Result<T>> result)
    {
        return result.IsSuccess 
            ? result.Value 
            : Result.Failure<T>(result.Error);
    }

    /// <summary>
    /// Taps into the result for inspection without changing it.
    /// Useful for debugging or logging both paths.
    /// </summary>
    public static Result<T> Tap<T>(
        this Result<T> result,
        Action<T> onSuccess,
        Action<string> onFailure)
    {
        if (result.IsSuccess)
            onSuccess(result.Value);
        else
            onFailure(result.Error);

        return result;
    }
}
