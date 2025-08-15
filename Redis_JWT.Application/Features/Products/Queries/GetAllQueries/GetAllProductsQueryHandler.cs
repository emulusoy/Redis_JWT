using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Queries.GetAll
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<List<ProductDto>>>
    {
        private readonly IRedisJwtContext _context;
        private readonly ICacheService _cache;
        private const string CacheKey = "products:all";

        public GetAllProductsQueryHandler(IRedisJwtContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Result<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cached = await _cache.GetAsync<List<ProductDto>>(CacheKey, cancellationToken);
                if (cached is not null) return Result<List<ProductDto>>.Success(cached);
            }
            catch
            {
                await _cache.RemoveAsync(CacheKey, cancellationToken);
            }

            var list = await _context.Products
                .OrderByDescending(p => p.CreatedTime)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(CacheKey, list, TimeSpan.FromMinutes(5), cancellationToken);
            return Result<List<ProductDto>>.Success(list);
        }
    }
}
