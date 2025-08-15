using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Common.Results;

namespace Redis_JWT.Application.Features.Auth.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<LoginResultDto>>
    {
        private readonly IRedisJwtContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenGenerator _jwt;

        public LoginQueryHandler(IRedisJwtContext db, IPasswordHasher hasher, IJwtTokenGenerator jwt)
        {
            _db = db;
            _hasher = hasher;
            _jwt = jwt;
        }

        public async Task<Result<LoginResultDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail, cancellationToken);
            if (user is null || !_hasher.Verify(user.PasswordHash, request.Password))
                return Result<LoginResultDto>.Failure(new("Auth.Invalid", "Invalid credentials"));

            var token = _jwt.GenerateToken(user);
            return Result<LoginResultDto>.Success(new(token, DateTime.UtcNow.AddMinutes(60)));
        }
    }
}
