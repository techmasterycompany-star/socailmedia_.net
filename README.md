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

Application and infrastructure registrations belong in their respective `DependencyInjection` classes. Infrastructure is intentionally unconfigured until a persistence or external-service choice is provided. Unexpected errors return a safe generic detail in the standard ProblemDetails error envelope.

## Packages

- API: `DotNetEnv` 3.2.0, `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.0, `Microsoft.AspNetCore.Authorization` 9.0.0, and `Microsoft.EntityFrameworkCore.Design` 9.0.0.
- Application: `AutoMapper` 14.0.0, `FluentValidation` 12.1.1, `FluentValidation.DependencyInjectionExtensions` 12.1.1, `Microsoft.AspNetCore.Http` 2.3.13, and `Microsoft.Extensions.Configuration.Abstractions` 9.0.0.
- Infrastructure: `BCrypt.Net-Next` 4.0.3, `CloudinaryDotNet` 1.29.3, `DotNetEnv` 3.2.0, `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 9.0.0, `Microsoft.EntityFrameworkCore.SqlServer` 9.0.0, `Microsoft.EntityFrameworkCore.Tools` 9.0.0, and the listed `Microsoft.Extensions.Configuration`, `.Abstractions`, `.EnvironmentVariables`, `.FileExtensions`, `.Json`, `Microsoft.Extensions.Http`, and `Microsoft.Extensions.Options.ConfigurationExtensions` packages at 9.0.0.
- Domain: no additional NuGet packages.

Package versions are pinned in their project files. EF Core Design and Tools references are private build-time dependencies.
