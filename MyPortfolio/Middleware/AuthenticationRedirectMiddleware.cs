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

            // تخطي المسارات المتعلقة بـ authentication والموارد الثابتة
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

            // الصفحة الرئيسية (Home/Index) متاحة للجميع بدون تسجيل دخول
            // فقط الصفحات الأخرى تحتاج تسجيل دخول
            var isHomePage = path == "/" ||
                             string.Equals(path, "/Home", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(path, "/Home/Index", StringComparison.OrdinalIgnoreCase);

            if (!isHomePage && context.User.Identity?.IsAuthenticated != true)
            {
                context.Response.Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(path)}");
                return;
            }

            await _next(context);
        }
    }
}
