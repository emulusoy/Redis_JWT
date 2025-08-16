
using System.Threading;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Common.Cache;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Queries.GetByIdQuery
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IRedisJwtContext _context;
        private readonly ICacheService _cache;

        public GetProductByIdQueryHandler(IRedisJwtContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var key = CacheKeys.ProductItem(request.Id);

            try
            {
                var cached = await _cache.GetAsync<ProductDto>(key, cancellationToken);
                if (cached is not null) return Result<ProductDto>.Success(cached);
            }
            catch
            {
                await _cache.RemoveAsync(key, cancellationToken); 
            }

            var dto = await _context.Products
                .Where(p => p.Id == request.Id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (dto is null)
                return Result<ProductDto>.Failure(new("Products.NotFound", "Product not found"));
            await _cache.SetAsync(key, dto, TimeSpan.FromMinutes(5), cancellationToken);
            return Result<ProductDto>.Success(dto);
        }
    }
}
