namespace MyPortfolio.Middleware
{
    public class AuthenticationRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationRedirectMiddleware> _logger;

        public AuthenticationRedirectMiddleware(RequestDelegate next, ILogger<AuthenticationRedirectMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            var publicPaths = new[]
            {
                "/signin", "/callback", "/auth", "/account",
                "/api", "/lib", "/css", "/js", "/Images", "/Files"
            };

            foreach (var prefix in publicPaths)
            {
                if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
                    (prefix.Length <= 4 && path.Contains(prefix, StringComparison.OrdinalIgnoreCase) && prefix.StartsWith("/")))
                {
                    await _next(context);
                    return;
                }
            }

            await _next(context);
        }
    }
}
