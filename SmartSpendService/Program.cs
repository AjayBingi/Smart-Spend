using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartSpendService.Data;
using SmartSpendService.Helpers;
using SmartSpendService.Repositories;
using SmartSpendService.Services;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// SQL file path
var sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SqlScript.sql");

// Register DatabaseHelper as singleton
builder.Services.AddSingleton(new DatabaseHelper(connectionString, sqlFilePath));

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
Console.WriteLine($"JWT Issuer: {jwtSettings.Issuer}, Audience: {jwtSettings.Audience}, Key: {jwtSettings.SecretKey}");

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<AISettings>(builder.Configuration.GetSection("AISettings"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully.");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Repositories
builder.Services.AddScoped<UserLibrary>();
builder.Services.AddScoped<UserProfileLibrary>();
builder.Services.AddScoped<ExpenseLibrary>();
builder.Services.AddScoped<BudgetLibrary>();
builder.Services.AddScoped<CategoryLibrary>();
builder.Services.AddScoped<AILibrary>();

// Services
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<BudgetService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<AnalyticsService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Swagger with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartSpend Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "Bearer" }},
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Initialize DB on startup
var db = app.Services.GetRequiredService<DatabaseHelper>();
db.Initialize();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();