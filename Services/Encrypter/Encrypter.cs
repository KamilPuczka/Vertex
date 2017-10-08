using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class Encrypter : IEncrypter
    {
        public string EncryptConnectionString(string hash)
        {
            return BCrypt.Net.BCrypt.HashPassword(hash);
        }

        public string HashConnectionString(string conString)
        {
            throw new NotImplementedException();
        }
    }
}
