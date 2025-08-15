using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis_JWT.Domain.Entities;

namespace Redis_JWT.Application.Abstractions
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, TimeSpan? lifetime = null);
    }
}
