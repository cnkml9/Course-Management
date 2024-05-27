using CourseService.API.Services;
using CourseService.API.Services.Video;
using CourseService.Application.Abstraction.Repositories;
using CourseService.Application.Abstraction.Services;
using CourseService.Infrastructure;
using CourseService.Infrastructure.Concretes.Repositories;
using CourseService.Infrastructure.Context;
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


//mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//logger
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
builder.Services.AddDependency();
builder.Services.AddScoped<IIdentityService,IdentityService>();
builder.Services.AddScoped<IVideoRepository,VideoRepository>();
builder.Services.AddScoped<IFileStorageService,FileStorageService>();   

builder.Services.AddHttpClient();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();



//jwtTok-authonication
builder.Services.AddDistributedMemoryCache(); 

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


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
            ValidateAudience = true, //oluþturulacak token deðerini kimlerin/sitelerin kullanacaðýný belirler
            ValidateIssuer = true, //oluþturulacak token deðerini kimin daðýttýðýný ifade edeceðimiz alandýr
            ValidateLifetime = true, //Token süresini kontrol eder doðrulama yeri
            ValidateIssuerSigningKey = true, //üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden seciry key verisisinin doðrulamasýdýr.



            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            //jwt ömrünü belirliyor 15 saniyelik token'ý 15saniye sonra etkisiz yapýyor ve eriþimi engelliyor-jwtRefreshToken yapýlandýrmasý için
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            //Log mekanýzmasý için
            NameClaimType = ClaimTypes.Name,  //JWT üzerinde Name claime karþýlýk gelen deðeri
                                             //User.Identity.Name propertysinden elde edebiliriz.
            //RoleClaimType = ClaimTypes.Role

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


//wwwroot kullanabilmek için eklenmeli
app.UseStaticFiles();

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
