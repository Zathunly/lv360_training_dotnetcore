using System.Net;
using System.Text.Json;
<<<<<<< HEAD
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient; // optional, for DB exceptions
=======
>>>>>>> 76414fe (Move DTOs + Enums to Domain)

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
<<<<<<< HEAD
            await _next(context); // Call next middleware
=======
            await _next(context); 
>>>>>>> 76414fe (Move DTOs + Enums to Domain)
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string message;

        switch (exception)
        {
<<<<<<< HEAD
            case SqlException: // DB exceptions
                status = HttpStatusCode.InternalServerError;
                message = "A database error occurred.";
                break;
            case KeyNotFoundException: // Not found
=======
            case KeyNotFoundException:
>>>>>>> 76414fe (Move DTOs + Enums to Domain)
                status = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case UnauthorizedAccessException:
                status = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            default:
                status = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        var result = JsonSerializer.Serialize(new
        {
            error = message,
            statusCode = (int)status
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(result);
    }
}
