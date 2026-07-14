using HomeSampling.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Module.Admin;
using Module.Auth;
using Module.Contact;
using Module.Patient;
using Module.Rider;
using Shared.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config   = builder.Configuration;
// ── CORS ────────────────────────────────────────────────────────
var allowedOrigins = config.GetSection("AllowedOrigins").Get<string[]>()!;
services.AddCors(opt => opt.AddPolicy("ReactApp", policy =>
    policy.WithOrigins(allowedOrigins)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()));

// ── JWT Authentication ───────────────────────────────────────────
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = config["Jwt:Issuer"],
            ValidAudience            = config["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
        };
    });

services.AddAuthorization();

// ── Module registrations ─────────────────────────────────────────
services.AddSharedInfrastructure(config);
services.AddModuleAuth(config);
services.AddModulePatient(config);
services.AddModuleAdmin(config);
services.AddModuleRider(config);
services.AddModuleContact(config);

// ── Core ASP.NET ─────────────────────────────────────────────────
services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new() { Title = "HomeSampling API", Version = "v1" });

    // Add JWT button to Swagger UI
    opt.AddSecurityDefinition("Bearer", new()
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Enter your JWT token here"
    });
    opt.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// ── Build ────────────────────────────────────────────────────────
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>(); // must be FIRST

app.UseCors("ReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ... (all your other code above remains exactly the same)

app.UseHttpsRedirection();
app.UseAuthentication(); // before Authorization
app.UseAuthorization();

app.MapControllers();

// ── TEST ENDPOINT ────────────────────────────────────────────────
app.MapGet("/api/test-db", (IConfiguration configuration) =>
{
    string? connectionString = configuration.GetConnectionString("DefaultConnection");

    using (Microsoft.Data.SqlClient.SqlConnection connection = new(connectionString))
    {
        try
        {
            connection.Open();
            using (Microsoft.Data.SqlClient.SqlCommand command = new("SELECT 1", connection))
            {
                command.ExecuteScalar();
            }

            return Results.Ok(new { status = "Success", message = "ADO.NET successfully connected to HomeSamplingDB!" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Database connection failed: {ex.Message}");
        }
    }
});
// ─────────────────────────────────────────────────────────────────

app.Run();
