using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Features.Auth.Commands.RegisterUser;
using Redis_JWT.Persistence.Auth;
using Redis_JWT.Persistence.Caching;
using Redis_JWT.Persistence.Context;
//case de belirtilen Global exception handling hatalarimizin ne oldugunu anlamak adina kulaniriz- 500 400 cod yerine isimlendirme diyebiliriz! bunun icin serilog kullandik bubir paket
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RedisJwtContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
//DB baglantisi bile mimariye uygun servisten geliyor  DI uyguladik az servisimiz oldugu icin program cs icinde kullandik!
builder.Services.AddScoped<IRedisJwtContext>(sp => sp.GetRequiredService<RedisJwtContext>());
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICacheService, DistributedCacheService>();


//case de belirtilen Global exception handling Logging 
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();


//Redis aktif et
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = builder.Configuration["Redis:Configuration"];
    opt.InstanceName = builder.Configuration["Redis:InstanceName"];
});

//mediatr kullanabilmek icin gerekli olan servis commanddan gelir
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());


//JWT ayarlari burada Audince Issuer Secret hepsini appsetting.json dan aliyor
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters 
        {
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };

        //loglama giris yaparken yetkiyi konstrol ederken burada hata mesajlarini veriyoruz kullanici anlasin diye log kismi ise bizim anlamamiz icin codu donuyor sadece
        opt.Events = new JwtBearerEvents
        {
            OnChallenge = async ctx =>
            {
                ctx.HandleResponse();
                var problem = Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Lutfen giris yapiniz - Please log in"
                );
                await problem.ExecuteAsync(ctx.HttpContext);
            },
            OnForbidden = async ctx =>
            {
                var problem = Results.Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Bu islem icin yetkiniz yok - You are not authorized for this operation"
                );
                await problem.ExecuteAsync(ctx.HttpContext);
            }
        };

    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

//---------------------------------------------------------------------------------------------------


//SWAGGER icin ust tarafa Authorize olup olmadiginiz kontrol etmemiz icin ekledik!
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Login yap | Tokeni Gir - Login | Enter Token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

//---------------------------------------------------------------------------------------------------
builder.Services.AddControllers();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

//loglama !!!!
app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext ctx, IHostEnvironment env, ILogger<Program> logger) =>
{
    var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
    logger.LogError(ex, "Beklenmedik bir hata olustu - Unhandled exception. TraceId={TraceId}", ctx.TraceIdentifier);
    return Results.Problem(
        statusCode: 500,
        title: "Beklenmedik bir hata olustu - Unexpected error"
    );
});

app.UseSerilogRequestLogging(); //middleware ekledik, bu middleware sayesinde gelen requestleri loglayabiliyoruz yani alinan istekleri loglamak icin!


app.UseHttpsRedirection();

app.UseAuthentication();   //auth icin eklenmesi gerekir!
app.UseAuthorization();    

app.MapControllers();

app.Run();

