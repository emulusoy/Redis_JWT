using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Commands.UpdateCommands
{
    public record UpdateProductCommand(int Id, string Name, string? Description, decimal Price, int Stock) : IRequest<Result>
    {
        ///Record yapisi ile bu classi immutable hale getiririr prop prop dan kurtulmak adina!
    }
}
