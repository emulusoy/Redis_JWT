# Redis_JWT (Onion + CQRS + JWT + Redis + EF Core/PostgreSQL)

Bu proje; **ölçeklenebilir servis mimarisi**, **JWT kimlik doğrulama**, **Redis cache**, **EF Core + PostgreSQL**, **Onion Architecture (Core–Application–Infrastructure–API)** ve **CQRS (MediatR)** kullanımını örnekler.

---

## Teknolojiler

| Teknoloji |
|---|
| .NET 8, C# |
| EF Core + PostgreSQL |
| Redis (StackExchange.Redis / IDistributedCache) |
| MediatR (CQRS: Command/Query + Handler) |
| JWT Authentication |
| Serilog |
| Swagger |

---

## Mimari (Onion)  
DOSYALAMA


Core
├─ Domain (Entities)
└─ Application
├─ Abstractions (Interfaces)
├─ Common (Results vs.)
└─ Features
└─ Auth, Products (Commands/Queries/Handlers, Dtos)
Infrastructure
└─ Persistence (Context, Auth, Caching)
Presentation
└─ WebApi (Controllers, Program.cs)

## Kurulum

- .NET 8 SDK  
- PostgreSQL  
- **Redis - Docker ile kullandım!**

**Postgres bağlantısı (appsettings.json)** – *burayı gizli tutmadım, incelemeniz için!*  

**Redis (Docker):**
```bash
docker run -d --name redis -p 6379:6379 redis

# Sağlık kontrolü:
docker exec -it redis redis-cli ping 

Migration + DB oluşturma

Migrationlar Infrastructure/Redis_JWT.Persistence projesinde, startup WebApi.

Add-Migration InitialCreate
Update-Database

Calistirmaya gec!


Logging & Global Exception

Serilog → Console

app.UseSerilogRequestLogging() → tüm istek/yanıtlar loglanır.

Global exception handler → Türkçe ProblemDetails (dev/prod davranışı ayarlı).

Notlar

SOLID: Servisler arayüzlerle soyutlandı (IJwtTokenGenerator, IPasswordHasher, ICacheService, IRedisJwtContext).

CQRS: MediatR ile Command/Query + Handler ayrımı.

Güvenlik: Prod’da HTTPS + güçlü Jwt:Secret kullanın.

Sık Sorunlar

401: Swagger’da Authorize yapmadınız veya token süresi doldu.

500/deserialize: Redis’te eski cache → DEL app:products:all veya FLUSHDB.

DB boş: Doğru connection string + migration update kontrolü.







