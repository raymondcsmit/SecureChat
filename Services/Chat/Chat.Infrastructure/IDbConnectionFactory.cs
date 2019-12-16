using System.Data;
using System.Threading.Tasks;

namespace Chat.Infrastructure
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> OpenConnectionAsync();
    }
}
