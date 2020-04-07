using System.Data;
using System.Threading.Tasks;

namespace Users.Infrastructure
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> OpenConnectionAsync();
    }
}
