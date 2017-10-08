using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using Services;
using System;
using System.Collections.Generic;
using System.Text;
using Vertex.Helpers;

namespace Vertex.Services
{
    public class GetOperations : IGetOperations
    {
        private ConfigGetterHelper _config { get; set; }
        private IEncrypter _encrypter { get; set; }
        public GetOperations(ConfigGetterHelper config, IEncrypter encrypter)
        {
            _encrypter = encrypter;
            _config = config;
        }
        public IEnumerable<T> Get<T>(string conString, string procName)
        {
            using (FbConnection mycon = new FbConnection(_encrypter.DecryptString(conString)))
            {
                return mycon.Query<T>(_config.GetSqlFrom(procName));
            }
        }
    }
}
