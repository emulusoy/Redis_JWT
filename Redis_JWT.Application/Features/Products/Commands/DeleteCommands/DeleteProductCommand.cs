using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Commands.DeleteCommands
{
    public record DeleteProductCommand(int Id)  :IRequest<Result>
    {
    }
}
