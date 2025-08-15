using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Commands.CreateCommands
{
    public record CreateProductCommand(string Name, string? Description, decimal Price, int Stock) : IRequest<Result<int>>
    {
    }
}
