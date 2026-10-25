using System.Text;
using APIWeb.Data;
using APIWeb.Models;
using APIWeb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Base de datos (MySQL)
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("ConexionMySQL")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'ConnectionStrings:ConexionMySQL'.");

builder.Services.AddDbContext<NorthwindContext>(options => options.UseMySQL(connectionString));
builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseMySQL(connectionString));

// ---------------------------------------------------------------------------
// Identity (usuarios locales)
// ---------------------------------------------------------------------------
builder.Services.AddIdentityCore<ApplicantUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;

        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

// ---------------------------------------------------------------------------
// JWT
// ---------------------------------------------------------------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.Key) || Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
{
    throw new InvalidOperationException("Jwt:Key debe tener al menos 32 caracteres.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenService>();

// ---------------------------------------------------------------------------
// MVC + CORS
// ---------------------------------------------------------------------------
builder.Services.AddControllers();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        if (allowedOrigins.Length == 0 || allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(allowedOrigins);
        }

        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

// ---------------------------------------------------------------------------
// OpenAPI (documento generado por Microsoft.AspNetCore.OpenApi)
// ---------------------------------------------------------------------------
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Token JWT obtenido en /api/Auth/login o /api/Auth/google."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["bearer"] = scheme;

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// Pipeline
// ---------------------------------------------------------------------------
app.MapOpenApi();

app.UseStaticFiles();

app.MapGet("/", () => Results.Json(new
{
    name = "Northwind OAuth API",
    status = "online",
    message = "Usa /api para consumir los endpoints. Consulta la documentación en /swagger/index.html",
    swagger = "/swagger/index.html",
    auth = new
    {
        register = "/api/Auth/register",
        login = "/api/Auth/login",
        google = "/api/Auth/google"
    }
}));

app.MapGet("/swagger", () => Results.Redirect("/swagger/index.html"));

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ---------------------------------------------------------------------------
// Migraciones + datos de ejemplo (si la base está disponible)
// ---------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<NorthwindContext>();
        var identityDb = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
        await DbSeeder.InitializeAsync(db, identityDb, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "No se pudo inicializar la base de datos. Revisa MySQL y la cadena 'ConexionMySQL'.");
    }
}

app.Run();
