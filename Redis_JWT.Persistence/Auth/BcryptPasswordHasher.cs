using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis_JWT.Application.Abstractions;

namespace Redis_JWT.Persistence.Auth
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)=> BCrypt.Net.BCrypt.HashPassword(password);
        public bool Verify(string passwordHash, string password)
        {
            if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(password))
                return false;
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

    }
}
