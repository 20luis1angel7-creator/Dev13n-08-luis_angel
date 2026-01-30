using Okane.Application;

namespace Okane.Tests;

public static class ResultExtensions
{
    public static void AssertOk(this Result result) => Assert.IsType<OkResult>(result);

    public static T AssertOk<T>(this Result<T> result)
    {
        var okResult = Assert.IsType<OkResult<T>>(result);
        return okResult.Value;
    }

    public static string AssertError(this Result result)
    {
        var errorResult = Assert.IsType<ErrorResult>(result);
        return errorResult.Message;
    }
    
    public static string AssertError<T>(this Result<T> result)
    {
        var errorResult = Assert.IsType<ErrorResult<T>>(result);//
        return errorResult.Message;
    }

    public static string AssertNotFound(this Result result)
    {
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        return notFoundResult.Message;
    }

    public static string AssertNotFound<T>(this Result<T> result)
    {
        var notFoundResult = Assert.IsType<NotFoundResult<T>>(result);
        return notFoundResult.Message;
    }
}