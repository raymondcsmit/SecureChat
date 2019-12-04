using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Associations.Infrastructure
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> OpenConnectionAsync();
    }
}
