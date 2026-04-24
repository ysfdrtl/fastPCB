using System.Text;
using FastPCB.API.Services;
using FastPCB.Data.Extensions;
using FastPCB.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FastPCB API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT tokenini `Bearer {token}` formatinda girin."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey tanimli degil.");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Database configuration
var connectionString = ResolveMySqlConnectionString(builder.Configuration);
builder.Services.AddFastPCBData(connectionString);

// Service registration
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IProjectLikeService, ProjectLikeService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IProjectFileStorage, LocalProjectFileStorage>();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// API version configuration
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            message = ex.Message,
            innerException = ex.InnerException?.Message,
            stackTrace = app.Environment.IsDevelopment() ? ex.StackTrace : null
        });
    }
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FastPCB API v1");
    c.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/swgger", () => Results.Redirect("/swagger"));

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

var configuredUploadsPath = app.Configuration["FileStorage:ProjectUploadsPath"];
var uploadsRoot = string.IsNullOrWhiteSpace(configuredUploadsPath)
    ? Path.Combine(app.Environment.ContentRootPath, "uploads", "projects")
    : Path.IsPathRooted(configuredUploadsPath)
        ? configuredUploadsPath
        : Path.Combine(app.Environment.ContentRootPath, configuredUploadsPath);
Directory.CreateDirectory(uploadsRoot);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsRoot),
    RequestPath = "/uploads/projects"
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static string ResolveMySqlConnectionString(IConfiguration configuration)
{
    var configuredConnectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? configuration["MYSQL_URL"]
        ?? configuration["DATABASE_URL"];

    if (string.IsNullOrWhiteSpace(configuredConnectionString))
    {
        throw new InvalidOperationException("ConnectionStrings:DefaultConnection, MYSQL_URL veya DATABASE_URL tanimli degil.");
    }

    if (Uri.TryCreate(configuredConnectionString, UriKind.Absolute, out var databaseUrl)
        && (databaseUrl.Scheme.Equals("mysql", StringComparison.OrdinalIgnoreCase)
            || databaseUrl.Scheme.Equals("mariadb", StringComparison.OrdinalIgnoreCase)))
    {
        var credentials = databaseUrl.UserInfo.Split(':', 2);
        var databaseName = databaseUrl.AbsolutePath.TrimStart('/');

        return new MySqlConnectionStringBuilder
        {
            Server = databaseUrl.Host,
            Port = (uint)(databaseUrl.IsDefaultPort ? 3306 : databaseUrl.Port),
            Database = Uri.UnescapeDataString(databaseName),
            UserID = credentials.Length > 0 ? Uri.UnescapeDataString(credentials[0]) : string.Empty,
            Password = credentials.Length > 1 ? Uri.UnescapeDataString(credentials[1]) : string.Empty,
            SslMode = MySqlSslMode.Preferred
        }.ConnectionString;
    }

    return configuredConnectionString;
}
