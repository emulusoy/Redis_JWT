using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Domain.Entities;

namespace Redis_JWT.Application.Abstractions
{
    public interface IRedisJwtContext
    {
        DbSet<User> Users { get; }
        DbSet<Product> Products { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
