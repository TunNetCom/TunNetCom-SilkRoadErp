namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

public static class ResultExtension
{
    public static ValidationProblem ToValidationProblem(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();

        }

        var errors = result.Errors
                .Select(e => e.Message)
                .ToArray();

        return TypedResults.ValidationProblem(
            title: "Bad Request",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            errors: new Dictionary<string, string[]>
            {
                { "errors" ,  errors  }
            });
    }


    public static ValidationProblem ToValidationProblem(this Result<int> result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();

        }

        var errors = result.Errors
                .Select(e => e.Message)
                .ToArray();

        return TypedResults.ValidationProblem(
            title: "Bad Request",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            errors: new Dictionary<string, string[]>
            {
                { "errors" ,  errors  }
            });
    }

    public static ValidationProblem ToValidationProblem(this Result<string> result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();

        }

        var errors = result.Errors
                .Select(e => e.Message)
                .ToArray();

        return TypedResults.ValidationProblem(
            title: "Bad Request",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            errors: new Dictionary<string, string[]>
            {
                { "errors" ,  errors  }
            });
    }
}

