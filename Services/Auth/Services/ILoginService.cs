using System.Threading.Tasks;
using Auth.Models;
using Microsoft.AspNetCore.Authentication;

namespace Auth.Services
{
    public interface ILoginService<T>
    {
        Task<User> SignInByUserNameAsync(string userName, string password, AuthenticationProperties properties, string authenticationMethod = null);
        Task SignOutAsync();
    }
}
