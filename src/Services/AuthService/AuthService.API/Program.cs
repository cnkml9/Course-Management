using AuthService.Application.Abstractions.Services;
using AuthService.Application.Abstractions.Services.Auth;
using AuthService.Application.Abstractions.Services.Identity;
using AuthService.Application.Abstractions.Services.Token;
using AuthService.Domain.Models;
using AuthService.Infrastructure.Services;
using AuthService.Infrastructure.Services.Identity;
using AuthService.Persistence;
using AuthService.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Token tabanlý kimlik doðrulama ekleyin
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new List<string>() { }
        }
    });
});

builder.Services.AddCors(opt => opt.AddDefaultPolicy(
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    ));




//logger-conf
builder.Services.AddSingleton<ILoggerService>(provider =>
{
    // Uygulamanýn çalýþma dizini alýnýyor
    var currentDirectory = Directory.GetCurrentDirectory();

    // Log dosyasýnýn tam yolu oluþturuluyor
    var logFilePath = Path.Combine(currentDirectory, "Logging", "app.log");

    // Log dosyasýnýn bulunduðu klasör kontrol ediliyor, yoksa oluþturuluyor
    var logDirectory = Path.GetDirectoryName(logFilePath);
    if (!Directory.Exists(logDirectory))
    {
        Directory.CreateDirectory(logDirectory);
    }

    // Eðer log dosyasý yoksa oluþturulup baþlangýç mesajý yazýlýyor
    if (!File.Exists(logFilePath))
    {
        File.WriteAllText(logFilePath, $"{DateTime.Now} - Log file created.\n");
    }

    return new FileLoggerService(logFilePath);
});


//serviceRegistration

builder.Services.AddPersistence(builder.Configuration);


builder.Services.AddScoped<ITokenHandler, AuthService.Infrastructure.Services.Token.TokenHandler>();
builder.Services.AddScoped<IAuthService, AuthService.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

//jwtTok-authonication

builder.Services.AddDistributedMemoryCache(); // Bellek tabanlý oturum yönetimi kullanmak için

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();//client'tan gele request neticesinde oluþturulan httpcontext nesnesine katmanlardak 

//identity

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, 
            ValidateIssuer = true, 
            ValidateLifetime = true, 
            ValidateIssuerSigningKey = true,



            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            //Log mekanýzmasý için
            NameClaimType = ClaimTypes.Name  
            

        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"Challenge: {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                Console.WriteLine($"Forbidden: {context.Response.StatusCode}");
                return Task.CompletedTask;
            }
        };
    });

//jwtTok-authentication

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
