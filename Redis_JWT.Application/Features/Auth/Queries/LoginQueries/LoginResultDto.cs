using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis_JWT.Application.Features.Auth.Queries.LoginQueries
{
    public sealed record LoginResultDto(string AccessToken, DateTime ExpiresAtUtc);
}
