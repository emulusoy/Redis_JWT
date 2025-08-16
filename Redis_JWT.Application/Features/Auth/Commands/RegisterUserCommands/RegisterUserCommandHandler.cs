using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Redis_JWT.Application.Abstractions;
using Redis_JWT.Application.Common.Results;
using Redis_JWT.Domain.Entities;

namespace Redis_JWT.Application.Features.Auth.Commands.RegisterUserCommands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IRedisJwtContext _db;
        private readonly IPasswordHasher _hasher;

        public RegisterUserCommandHandler(IRedisJwtContext db, IPasswordHasher hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exists = await _db.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);
            if (exists) return Result.Failure(new("Auth.Duplicate", "Username or Email already in use"));

            var user = new User { Username = request.Username, Email = request.Email, PasswordHash = _hasher.Hash(request.Password) };
            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
