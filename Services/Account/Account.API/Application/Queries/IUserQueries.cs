using System.Threading.Tasks;
using Account.API.Dtos;

namespace Account.API.Application.Queries
{
    public interface IUserQueries
    {
        Task<UserDto> GetUserByIdAsync(string id);
    }
}
