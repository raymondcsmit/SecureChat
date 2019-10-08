using System.Threading.Tasks;
using Auth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Auth.Services
{
    public class IdentityLoginService : ILoginService<User>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IdentityLoginService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> SignInByUserNameAsync(string userName, string password, AuthenticationProperties properties, string authenticationMethod = null)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                // issue authentication cookie with subject ID and username
                await _signInManager.SignInAsync(user, properties, authenticationMethod);
                return user;
            }

            return null;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
