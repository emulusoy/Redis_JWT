using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Products.Queries.GetAllQueries
{
    public class GetAllProductsQuery():IRequest<Result<List<ProductDto>>>
    {
    }
}
