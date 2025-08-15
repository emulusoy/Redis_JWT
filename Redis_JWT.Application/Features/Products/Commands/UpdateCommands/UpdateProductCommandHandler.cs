using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Application.Abstractions;
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
            if (entity is null) return Result.Failure(new("Products.NotFound", "Product not found"));

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Price = request.Price;
            entity.Stock = request.Stock;
            entity.UpdatedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync("products:all", cancellationToken);
            return Result.Success();
        }
    }
}
