using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace TaskAPI
{
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtAuthenticationMiddleware(RequestDelegate next, TokenValidationParameters tokenValidationParameters)
        {
            _next = next;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) ||
                !authorizationHeader.Any() ||
                !authorizationHeader[0].StartsWith("Bearer "))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var token = authorizationHeader[0].Substring("Bearer ".Length);

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // Validate the token and extract claims
                var claimsPrincipal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);
                context.User = claimsPrincipal;
            }
            catch (SecurityTokenException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await _next(context);
        }
    }

    // Extension method to register the middleware
    public static class JwtAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app, TokenValidationParameters tokenValidationParameters)
        {
            return app.UseMiddleware<JwtAuthenticationMiddleware>(tokenValidationParameters);
        }
    }
}
