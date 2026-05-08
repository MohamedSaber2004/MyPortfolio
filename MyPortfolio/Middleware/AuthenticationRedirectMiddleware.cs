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
            // استثناء طلبات Google authentication callback
            var path = context.Request.Path.Value ?? "";

            // تخطي أي مسارات متعلقة بـ authentication
            if (path.Contains("/signin", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/callback", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/auth", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/account", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/api", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/Images", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/Files", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // إذا كان المستخدم يحاول الوصول إلى Home/Index بدون تسجيل
            if (path == "/" && context.User.Identity?.IsAuthenticated != true)
            {
                context.Response.Redirect("/Home/Welcome");
                return;
            }

            await _next(context);
        }
    }
}
