using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Auth.Queries.LoginQueries
{
    public sealed record LoginQuery(string UsernameOrEmail, string Password) : IRequest<Result<LoginResultDto>>;

}
