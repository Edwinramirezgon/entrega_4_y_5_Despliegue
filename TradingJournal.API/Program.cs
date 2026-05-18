using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using TradingJournal.API.Data;
using TradingJournal.API.Helpers;
using TradingJournal.Shared.Entities;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trading Journal API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
 Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
 Example: 'Bearer 12345abcdef'<br /> <br />",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
 {
 {
 new OpenApiSecurityScheme
 {
 Reference = new OpenApiReference
 {
 Type = ReferenceType.SecurityScheme,
 Id = "Bearer"
 },
 Scheme = "oauth2",
 Name = "Bearer",
 In = ParameterLocation.Header,
 },
 new List<string>()
 }
 });
});


var connectionString = builder.Environment.IsProduction()
    ? builder.Configuration.GetConnectionString("Connectionwhitparams")
        ?? throw new InvalidOperationException("Connection string 'Connectionwhitparams' was not found.")
    : builder.Configuration.GetConnectionString("WindowsSecurity")
        ?? throw new InvalidOperationException("Connection string 'WindowsSecurity' was not found.");

builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(connectionString));


builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    x.SignIn.RequireConfirmedEmail = true;
    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = true;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = true;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = true;
    x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    x.Lockout.MaxFailedAccessAttempts = 3;
    x.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();


builder.Services.AddScoped<IUserHelper, UserHelper>();

builder.Services.AddScoped<IMailHelper, MailHelper>();

builder.Services.AddScoped<IFileStorage, FileStorage>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
    ClockSkew = TimeSpan.Zero
});



builder.Services.AddTransient<SeedDb>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

SeedData(app);

static void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory!.CreateScope())
    {
        try
        {
            var service = scope.ServiceProvider.GetService<SeedDb>();
            service!.SeedAsync().Wait();
        }
        catch
        {
        }
    }
}

if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors(x => x
    .WithOrigins("https://tradingjournalweb-hyh0hna6dweka3bh.canadacentral-01.azurewebsites.net")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok("OK"));
app.MapGet("/pipeline-check", () => Results.Content("""
<!DOCTYPE html>
<html lang=\"es\">
<head>
  <meta charset=\"utf-8\" />
  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />
  <title>Pipeline OK</title>
  <style>
    body { font-family: Arial, sans-serif; background: #0f172a; color: #e2e8f0; margin: 0; }
    .wrap { max-width: 720px; margin: 6rem auto; padding: 2rem; background: #111827; border: 1px solid #1f2937; border-radius: 16px; box-shadow: 0 10px 30px rgba(0,0,0,.35); }
    .badge { display: inline-block; padding: .35rem .75rem; background: #16a34a; color: #0b1220; border-radius: 999px; font-weight: 700; }
    h1 { margin: 1rem 0 .5rem; font-size: 1.8rem; }
    .meta { margin-top: 1rem; font-size: .95rem; color: #94a3b8; }
  </style>
</head>
<body>
  <div class=\"wrap\">
    <span class=\"badge\">OK</span>
    <h1>Despliegue continuo exitoso ✅</h1>
    <div>Integración continua y despliegue continuo funcionando correctamente.</div>
    <div class=\"meta\">Actualizado: """ + DateTime.UtcNow.ToString("u") + """</div>
  </div>
</body>
</html>
""", "text/html"));

app.Run();
