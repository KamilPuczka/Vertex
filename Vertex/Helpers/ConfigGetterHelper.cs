using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vertex.Helpers
{
    public class ConfigGetterHelper
    {
        public IConfigurationRoot _config { get; set; }
        public ConfigGetterHelper(IConfigurationRoot config)
        {
            _config = config;
        }
        public string GetSqlFrom(string key)
        {
            return _config.GetSection("Sql").GetValue<string>(key);
        }
    }
}
