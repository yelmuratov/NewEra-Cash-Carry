using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NewEra_Cash___Carry.Infrastructure.Data;
using System.Threading.Tasks;

namespace NewEra_Cash___Carry.API.Middlewares
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public TokenBlacklistMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                // Create a new scope for the DbContext
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<RetailOrderingSystemDbContext>();

                // Check if the token is blacklisted
                if (await dbContext.BlacklistedTokens.AnyAsync(bt => bt.Token == token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token is invalid or expired.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
