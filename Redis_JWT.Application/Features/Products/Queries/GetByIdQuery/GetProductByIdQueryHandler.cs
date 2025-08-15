
using System.Threading;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Queries.GetByIdQuery
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IRedisJwtContext _context;

        public GetProductByIdQueryHandler(IRedisJwtContext context)
        {
            _context = context;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
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

            return dto is null
                ? Result<ProductDto>.Failure(new("Products.NotFound", "Product not found"))
                : Result<ProductDto>.Success(dto);
        }
    }
}
