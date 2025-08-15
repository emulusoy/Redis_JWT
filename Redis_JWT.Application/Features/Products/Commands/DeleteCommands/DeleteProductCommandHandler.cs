using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Commands.DeleteCommands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly IRedisJwtContext _context;
        private readonly ICacheService _cache;

        public DeleteProductCommandHandler(IRedisJwtContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (entity is null) return Result.Success(); // record yaptiysan silme islemi boyle olacak!
            _context.Products.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync("products:all", cancellationToken);
            return Result.Success();
        }
    }
}
