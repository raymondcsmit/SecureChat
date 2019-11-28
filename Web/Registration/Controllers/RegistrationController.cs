using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Registration.Exceptions;
using Registration.Models;
using Registration.Services;

namespace Registration.Controllers
{
    [Route("registration")]
    public class RegistrationController : Controller
    {
        private readonly IUsersClient _usersClient;
        private readonly IConfiguration _configuration;

        public RegistrationController(IUsersClient usersClient, IConfiguration configuration)
        {
            _usersClient = usersClient;
            _configuration = configuration;
        }

        [HttpGet("", Name = nameof(RegisterGet))]
        public IActionResult RegisterGet([FromQuery] string loginUrl)
        {
            if (loginUrl != null)
            {
                TempData["LoginUrl"] = loginUrl;
            }

            return View();
        }

        [HttpPost("", Name = nameof(RegisterPost))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost([FromForm] RegistrationFormDto registrationForm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var user = await _usersClient.CreateUserAsync(registrationForm);
                TempData["ConfirmationEmail"] = user.Email;
                return RedirectToAction(nameof(RegisterConfirmationGet));
            }
            catch (ApiException e)
            {
                TempData["Errors"] = e.Errors;
            }

            return RedirectToAction(nameof(RegisterGet));

        }

        [HttpGet("confirmation", Name = nameof(RegisterConfirmationGet))]
        public IActionResult RegisterConfirmationGet()
        {
            ViewData["LoginUrl"] = TempData["LoginUrl"] ?? $"{_configuration["AuthUrl"]}/account/login";
            return View();
        }
    }
}