using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Registration.Exceptions;
using Registration.Models;
using Registration.Services;

namespace Registration.Controllers
{
    [Route("email-confirmation")]
    public class EmailConfirmationController : Controller
    {
        private readonly IAccountClient _accountClient;

        public EmailConfirmationController(IAccountClient accountClient)
        {
            _accountClient = accountClient;
        }

        [HttpGet("", Name = nameof(ConfirmEmailGet))]
        public async Task<IActionResult> ConfirmEmailGet([FromQuery] EmailConfirmationDto emailConfirmationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _accountClient.ConfirmEmailAsync(emailConfirmationDto);
                return RedirectToAction(nameof(ConfirmEmailConfirmationGet));
            }
            catch (ApiException)
            {
                return RedirectToAction(nameof(ConfirmEmailFailureGet));
            }
        }

        [HttpGet("confirmation", Name = nameof(ConfirmEmailConfirmationGet))]
        public IActionResult ConfirmEmailConfirmationGet()
        {
            return View();
        }

        [HttpGet("failure", Name = nameof(ConfirmEmailFailureGet))]
        public IActionResult ConfirmEmailFailureGet()
        {
            return View();
        }
    }
}