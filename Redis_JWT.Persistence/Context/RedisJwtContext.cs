using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Domain.Entities;

namespace Redis_JWT.Persistence.Context
{
    public class RedisJwtContext(DbContextOptions<RedisJwtContext> options, IConfiguration configuration) : DbContext(options), IRedisJwtContext
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<Product> Products => Set<Product>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}
