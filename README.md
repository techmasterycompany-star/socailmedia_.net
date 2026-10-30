# Social_Media_Platform

A .NET 9 ASP.NET Core Web API starter with a four-project layered architecture. It contains no business endpoints, domain data, persistence provider, authentication provider, or external-service integrations.

## Projects

```text
Social_Media_Platform/
├── Social_Media_Platform.sln
└── src/
    ├── Social_Media_Platform.API/
    ├── Social_Media_Platform.Application/
    ├── Social_Media_Platform.Domain/
    └── Social_Media_Platform.Infrastructure/
```

Project references flow inward:

- API -> Application, Infrastructure
- Application -> Domain
- Infrastructure -> Application, Domain
- Domain -> no projects

## Run

Use the .NET 9 SDK.

```powershell
dotnet build .\Social_Media_Platform.sln
dotnet run --project .\src\Social_Media_Platform.API\Social_Media_Platform.API.csproj
```

The API exposes `GET /health` with the standard success/data response envelope.

## Configuration and secrets

ASP.NET Core loads `appsettings.json`, the matching `appsettings.{Environment}.json`, and environment variables. Use double underscores for nested environment keys. Keep secret values in environment variables or .NET User Secrets; do not commit secrets to settings files.

Application and infrastructure registrations belong in their respective `DependencyInjection` classes. Unexpected errors return a safe generic detail in the standard ProblemDetails error envelope.

## Database and migrations

The Infrastructure project owns the SQL Server `Social_Media_PlatformDbContext`, EF Core migrations, and an idempotent `DbSeeder` for the initial role records. On API startup, pending migrations are applied before the seeder runs; failures are allowed to fail startup. For local development, put the connection string in a root `.env` file using the ASP.NET Core environment-variable key:

```dotenv
ConnectionStrings__DefaultConnection="Server=<server>;Database=<database>;User Id=<user>;Password=<password>;TrustServerCertificate=True"
```

The API and EF Core design-time factory load `.env` from the solution root or a parent directory. The environment value overrides `ConnectionStrings:DefaultConnection` in appsettings. Keep `.env` private; it is ignored by Git. Production deployments should use their secret/environment configuration provider instead. Automatic startup migrations can cause concurrent DDL attempts when multiple API instances start together, so coordinate deployments to ensure only one instance performs startup migration at a time.

Startup also seeds two administrator accounts. Add these values to the same private `.env` file; do not commit the file or share its credentials:

```dotenv
SeedUsers__Admin1__Email="<first-admin-email>"
SeedUsers__Admin1__Username="<first-admin-username>"
SeedUsers__Admin1__Password="<first-admin-password>"
SeedUsers__Admin2__Email="<second-admin-email>"
SeedUsers__Admin2__Username="<second-admin-username>"
SeedUsers__Admin2__Password="<second-admin-password>"
```

Both passwords are stored as BCrypt hashes. The seeder does not reset credentials on subsequent starts; already seeded users are left unchanged. Keep the `.env` file accessible only to trusted users on the machine—ignoring a file in Git prevents committing it, but does not itself encrypt or restrict local file access.

Run EF commands from the solution root, with Infrastructure as the migrations project and API as the startup project:

```powershell
$env:ConnectionStrings__DefaultConnection = "<connection-string>"
dotnet ef migrations add InitialSchema --project .\src\Social_Media_Platform.Infrastructure\Social_Media_Platform.Infrastructure.csproj --startup-project .\src\Social_Media_Platform.API\Social_Media_Platform.API.csproj --output-dir Data\Migrations
dotnet ef database update --project .\src\Social_Media_Platform.Infrastructure\Social_Media_Platform.Infrastructure.csproj --startup-project .\src\Social_Media_Platform.API\Social_Media_Platform.API.csproj
```

## Packages

- API: `DotNetEnv` 3.2.0, `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.0, `Microsoft.AspNetCore.Authorization` 9.0.0, and `Microsoft.EntityFrameworkCore.Design` 9.0.0.
- Application: `AutoMapper` 14.0.0, `FluentValidation` 12.1.1, `FluentValidation.DependencyInjectionExtensions` 12.1.1, `Microsoft.AspNetCore.Http` 2.3.13, and `Microsoft.Extensions.Configuration.Abstractions` 9.0.0.
- Infrastructure: `BCrypt.Net-Next` 4.0.3, `CloudinaryDotNet` 1.29.3, `DotNetEnv` 3.2.0, `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 9.0.0, `Microsoft.EntityFrameworkCore.SqlServer` 9.0.0, `Microsoft.EntityFrameworkCore.Tools` 9.0.0, and the listed `Microsoft.Extensions.Configuration`, `.Abstractions`, `.EnvironmentVariables`, `.FileExtensions`, `.Json`, `Microsoft.Extensions.Http`, and `Microsoft.Extensions.Options.ConfigurationExtensions` packages at 9.0.0.
- Domain: no additional NuGet packages.

Package versions are pinned in their project files. EF Core Design and Tools references are private build-time dependencies.
