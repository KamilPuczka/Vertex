using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Vertex.Helpers;

namespace Vertex.Services
{
    public class GetOperations : IGetOperations
    {
        private ConfigGetterHelper _config { get; set; }
        public GetOperations(ConfigGetterHelper config)
        {
            config = _config;
        }
        public IEnumerable<T> Get<T>(string conString, string procName)
        {
            using (FbConnection mycon = new FbConnection(conString))
            {
                return mycon.Query<T>(_config.GetSqlFrom(procName));
            }
        }
    }
}
