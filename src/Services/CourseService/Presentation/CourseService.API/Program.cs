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
    // Token tabanl� kimlik do�rulama ekleyin
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
    // Uygulaman�n �al��ma dizini al�n�yor
    var currentDirectory = Directory.GetCurrentDirectory();

    // Log dosyas�n�n tam yolu olu�turuluyor
    var logFilePath = Path.Combine(currentDirectory, "Logging", "app.log");

    // Log dosyas�n�n bulundu�u klas�r kontrol ediliyor, yoksa olu�turuluyor
    var logDirectory = Path.GetDirectoryName(logFilePath);
    if (!Directory.Exists(logDirectory))
    {
        Directory.CreateDirectory(logDirectory);
    }

    // E�er log dosyas� yoksa olu�turulup ba�lang�� mesaj� yaz�l�yor
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
            ValidateAudience = true, //olu�turulacak token de�erini kimlerin/sitelerin kullanaca��n� belirler
            ValidateIssuer = true, //olu�turulacak token de�erini kimin da��tt���n� ifade edece�imiz aland�r
            ValidateLifetime = true, //Token s�resini kontrol eder do�rulama yeri
            ValidateIssuerSigningKey = true, //�retilecek token de�erinin uygulamam�za ait bir de�er oldu�unu ifade eden seciry key verisisinin do�rulamas�d�r.



            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            //jwt �mr�n� belirliyor 15 saniyelik token'� 15saniye sonra etkisiz yap�yor ve eri�imi engelliyor-jwtRefreshToken yap�land�rmas� i�in
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            //Log mekan�zmas� i�in
            NameClaimType = ClaimTypes.Name,  //JWT �zerinde Name claime kar��l�k gelen de�eri
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


//wwwroot kullanabilmek i�in eklenmeli
app.UseStaticFiles();

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
