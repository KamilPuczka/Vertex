using System.Collections.Generic;

namespace Vertex.Services
{
    public interface IGetOperations
    {
        IEnumerable<T> Get<T>(string conString, string procName);
    }
}