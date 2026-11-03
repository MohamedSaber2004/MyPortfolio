using System.Reflection;
using MyPortfolio.Helpers;
using MyPortfolio.Middleware;
using Serilog;

namespace MyPortfolio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = builder.Environment;

            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null) builder.Configuration.AddUserSecrets(appAssembly, optional: true);
            }

            builder.Configuration.AddEnvironmentVariables().AddCommandLine(args);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateBootstrapLogger();

            // Add services to the container.
            #region Dependency Injection Container
            builder.Services.AddResponseCompression(options =>
            {
                // The site is served over HTTPS only; compression stays off there unless asked for.
                options.EnableForHttps = true;
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<PortfolioDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:Connection"]);
            });

            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<PortfolioDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddHttpContextAccessor(); // needed to read current user

            builder.Services.AddTransient<IMailService,MailService>();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.Secure = CookieSecurePolicy.Always;
            });

            var authBuilder = builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            });

            var googleSection = builder.Configuration.GetSection("Authentication:Google");
            var googleClientId = googleSection["ClientId"];
            var googleClientSecret = googleSection["ClientSecret"];
            if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
            {
                authBuilder.AddGoogle(options =>
                {
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    // Google can call back with a spent or missing state cookie (aborted consent,
                    // blocked cookies); that throws, so land the visitor back on the login page.
                    options.Events.OnRemoteFailure = ctx =>
                    {
                        ctx.HandleResponse();
                        ctx.HttpContext.Response.Redirect("/Account/Login");
                        return Task.CompletedTask;
                    };
                });
            }

            builder.Services.AddAutoMapper(m => m.AddProfile(new mappingProfiles()));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IContactService,ContactService>();
            builder.Services.AddScoped<IEducationService, EducationService>();
            builder.Services.AddScoped<IExperienceService, ExperienceService>();
            builder.Services.AddScoped<ISocialLinkService, SocialLinkService>();
            builder.Services.AddScoped<ISkillService, SkillService>();
            builder.Services.AddScoped<ICvDocumentService, CvDocumentService>();
            #endregion

            var app = builder.Build();

            IconSprite.Load(app.Environment);

            // Middlewares - PipeLines
            #region MiddleWares - PipeLines
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseResponseCompression();

            // Design-system assets arrive with a content fingerprint (?v=…) from asp-append-version,
            // so a year of immutable caching is safe; canonical and uploaded paths keep revalidating.
            app.Use((context, next) =>
            {
                var path = context.Request.Path.Value ?? string.Empty;
                if (context.Request.Query.ContainsKey("v") &&
                    (path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase) ||
                     path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase) ||
                     path.StartsWith("/fonts/", StringComparison.OrdinalIgnoreCase) ||
                     path.StartsWith("/assets/", StringComparison.OrdinalIgnoreCase)))
                {
                    context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
                }

                return next();
            });

            app.UseHttpsRedirection();

            // Static assets are served before auth so CSS/JS/images never touch the identity pipeline.
            app.MapStaticAssets();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            // 4xx/5xx without a body is dead weight for a visitor: re-execute into the styled error page.
            // Placed after auth so the re-executed /Home/Error still knows who is asking.
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");

            // Custom middleware for authentication redirect
            app.UseMiddleware<AuthenticationRedirectMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            #endregion

            app.Run();
        }
    }
}
