using lv360_training.Domain.Services.Redis;

public class SessionRenewMiddleware
{
    private readonly RequestDelegate _next;

    public SessionRenewMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRedisSessionService sessionService)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var sessionId = context.User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
            if (!string.IsNullOrEmpty(sessionId))
            {
                await sessionService.RenewSessionAsync(sessionId);
            }
        }

        await _next(context);
    }
}
