using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
   public interface IEncrypter
    {
        string HashConnectionString(string conString);
        string EncryptConnectionString(string hash);
    }
}
