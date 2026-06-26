# AGENTS.md — MyPortfolio

## Architecture

3-layer .NET 9 clean architecture (no test projects):
- **MyPortfolio/** — ASP.NET Core MVC web app (Controllers, Views, Models/ViewModels, Helpers, Middleware)
- **BusinessLogicLayer/** — Services (Interfaces/, Classes/, Implementations/), DTOs, AutoMapper Profiles
- **DataAccessLayer/** — EF Core DbContext, Migrations, entity Models, Repositories (Interfaces/, Classes/)

Dependency chain: `MyPortfolio → BusinessLogicLayer → DataAccessLayer`

## Commands

```bash
dotnet build                           # build all projects
dotnet run --project MyPortfolio       # start dev server
dotnet ef database update --project DataAccessLayer --startup-project MyPortfolio
```

Run the https profile: `dotnet run --project MyPortfolio --launch-profile https`

## Key code details

- **Connection string key**: `ConnectionStrings:Connection` (not `DefaultConnection` — README is outdated)
- **Program.cs** clears default config sources, loads `appsettings.json` + `appsettings.{env}.json`, then user secrets (only Dev/Test), env vars, CLI args
- **Services**: mail (`IMailService`/`MailService`) registered `AddTransient`; all domain services (`I*Service` → `*Service`) registered `AddScoped`
- **Auth**: ASP.NET Core Identity with custom `User`/`Role` entities; Google OAuth configured; cookie login path `/Account/Login`, 30-day expiry
- **Custom middleware**: `AuthenticationRedirectMiddleware` (registered after `UseAuthorization`)
- **Cookie policy**: `SameSiteMode.Unspecified`, `CookieSecurePolicy.Always`
- **ORM**: Entity Framework Core 9 with SQL Server; migrations in `DataAccessLayer/Data/Migrations/`
- **Mapping**: AutoMapper profile in `BusinessLogicLayer/Profiles/mappingProfiles.cs`
- **Client libs**: managed via libman (`libman.json`) — bootstrap 5.3.7 to `wwwroot/lib/bootstrap/`

## Repo conventions

- Repository pattern: interfaces in `DataAccessLayer/Repositories/Interfaces/`, implementations in `Classes/`
- View models in `MyPortfolio/Models/`, DTOs in `BusinessLogicLayer/DTos/`, entities in `DataAccessLayer/Models/`
- App settings (`appsettings*.json`) and publish profiles are gitignored — check `.gitignore` before committing
- No tests, no CI, no linting/formatting config exists
