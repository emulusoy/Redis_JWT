using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Common.Cache;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Commands.UpdateCommands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
    {
        private readonly IRedisJwtContext _context;
        private readonly ICacheService _cache;

        public UpdateProductCommandHandler(IRedisJwtContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (entity is null)
                return Result.Failure(new("Products.NotFound", "Product not found"));

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Price = request.Price;
            entity.Stock = request.Stock;

            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(CacheKeys.ProductsAll, cancellationToken);
            var itemKey = CacheKeys.ProductItem(request.Id);
            await _cache.RemoveAsync(itemKey, cancellationToken);
            var dto = new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Stock = entity.Stock
            };
            await _cache.SetAsync(itemKey, dto, TimeSpan.FromMinutes(5), cancellationToken);

            return Result.Success();
        }
    }
}
