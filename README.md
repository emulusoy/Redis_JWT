

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
```bash```
docker run -d --name redis -p 6379:6379 redis

# Sağlık kontrolü:
docker exec -it redis redis-cli ping 

---##Migration + DB oluşturma

---##Migrationlar Infrastructure/Redis_JWT.Persistence projesinde, startup WebApi.

Add-Migration InitialCreate
Update-Database

---##Calistirmaya gec!
Projeyi çalıştırın (Swagger açılır)

Swagger sağ üst Authorize → Login’den aldığınız token (eyJ… ) değerini girin

Ürün listeleme → Redis cache üzerinden gelir (TTL/Key yapılandırması koddadır)



##Logging & Global Exception

Serilog 

app.UseSerilogRequestLogging() → tüm istek/yanıtlar loglanır.

Global exception handler → Türkçe ProblemDetails 

Notlar

SOLID: Servisler arayüzlerle soyutlandı (IJwtTokenGenerator, IPasswordHasher, ICacheService, IRedisJwtContext).

CQRS: MediatR ile Command/Query + Handler ayrımı.

Güvenlik: Prod’da HTTPS + güçlü Jwt:Secret kullanın.

Sık Sorunlar

401: Swagger’da Authorize yapmadınız veya token süresi doldu.

500/deserialize: Redis’te eski cache → DEL app:products:all veya FLUSHDB.

DB boş: Doğru connection string + migration update kontrolü.

LOGLAMA KISMI FOTOLAR Status 200 Statuss 401 icin 
<img width="2511" height="1195" alt="Screenshot 2025-08-16 181211" src="https://github.com/user-attachments/assets/7205da83-21ed-4e3a-b89c-6e4600ee97c3" />
<img width="2540" height="1182" alt="Screenshot 2025-08-16 181017" src="https://github.com/user-attachments/assets/c1b63dc2-efa4-4615-b24d-684bf50329eb" />







