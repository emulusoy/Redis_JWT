//using System.Text;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Redis_JWT.Application.Abstractions;
//using Redis_JWT.Application.Features.Auth.Commands.RegisterUser;
//using Redis_JWT.Persistence.Auth;
//using Redis_JWT.Persistence.Caching;
//using Redis_JWT.Persistence.Context;
//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<RedisJwtContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
//builder.Services.AddScoped<IRedisJwtContext>(x => x.GetRequiredService<RedisJwtContext>());


////Servislerimizi programin gormesi icin ekleme
//builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
//builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
//builder.Services.AddScoped<ICacheService, DistributedCacheService>();
//builder.Services.AddAuthorization();
////mediatr kullanabilmek icin gerekli olan servis commanddan gelir
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());

////JWT ayarlari burada Audince Issuer Secret hepsini appsetting.json dan aliyor
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
//{
//    opt.RequireHttpsMetadata = true;//https gerekli olsun demek
//    opt.TokenValidationParameters = new TokenValidationParameters //diger parametreler
//    {
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ClockSkew = TimeSpan.Zero,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//    };

//});
////Redisi aktif edelim

//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration["Redis:Configuration"];
//    options.InstanceName = builder.Configuration["Redis:InstanceName"];
//});




//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthentication();

//app.UseAuthorization();
//app.MapControllers();

//app.Run();

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Features.Auth.Commands.RegisterUser;
using Redis_JWT.Persistence.Auth;
using Redis_JWT.Persistence.Caching;
using Redis_JWT.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<RedisJwtContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddScoped<IRedisJwtContext>(sp => sp.GetRequiredService<RedisJwtContext>());

// Uygulama servisleri
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = builder.Configuration["Redis:Configuration"];
    opt.InstanceName = builder.Configuration["Redis:InstanceName"];
});
builder.Services.AddScoped<ICacheService, DistributedCacheService>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());

// AuthN / AuthZ
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var secret = builder.Configuration["Jwt:Secret"]!;
        opt.RequireHttpsMetadata = false; // dev için false; prod'da true

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });
builder.Services.AddAuthorization();

// Swagger + Bearer şeması (Authorize butonu + header ekleme)
builder.Services.AddEndpointsApiExplorer();
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
        Description = "Login sonrası aldığın accessToken'ı buraya yapıştır (sadece eyJ…; 'Bearer' yazmana gerek yok)."
    });

    // Tüm endpoint’lere Bearer güvenliği uygula (Authorize’a tıklayınca header eklenir)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        // Token'ı tarayıcıda tut: sayfayı yenileyince de header ekli kalsın
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();   // önce
app.UseAuthorization();    // sonra

app.MapControllers();

app.Run();

