
using MediatR;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Common.Cache;
using Redis_JWT.Application.Common.Results;
using Redis_JWT.Domain.Entities;

namespace Redis_JWT.Application.Features.Products.Commands.CreateCommands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
    {
        private readonly IRedisJwtContext _context;
        private readonly ICacheService _cache;

        public CreateProductCommandHandler(IRedisJwtContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var entity = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock
            };

            _context.Products.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync(CacheKeys.ProductsAll, cancellationToken);
            var dto = new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Stock = entity.Stock
            };
            await _cache.SetAsync(CacheKeys.ProductItem(entity.Id), dto, TimeSpan.FromMinutes(5), cancellationToken);

            return Result<int>.Success(entity.Id);
        }
    }
}
