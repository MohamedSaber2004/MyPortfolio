
namespace MyPortfolio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Dependency Injection Container
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

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddGoogle(options =>
            {
                IConfigurationSection configurationBuilder = builder.Configuration.GetSection("Authentication:Google");
                options.ClientId = configurationBuilder["ClientId"]!;
                options.ClientSecret = configurationBuilder["ClientSecret"]!;
            });

            builder.Services.AddAutoMapper(m => m.AddProfile(new mappingProfiles()));

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IContactService,ContactService>();
            builder.Services.AddScoped<IEducationService, EducationService>();
            builder.Services.AddScoped<IExperienceService, ExperienceService>();
            builder.Services.AddScoped<ISocialLinkService, SocialLinkService>();
            builder.Services.AddScoped<ISkillService, SkillService>();
            #endregion

            var app = builder.Build();

            // Middlewares - PipeLines
            #region MiddleWares - PipeLines
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.UseStaticFiles();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            #endregion

            app.Run();
        }
    }
}
