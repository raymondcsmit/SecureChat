using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Registration.Exceptions;
using Registration.Models;
using Registration.Services;

namespace Registration.Controllers
{
    [Route("password-reset")]
    public class PasswordResetController : Controller
    {
        private readonly IAccountClient _accountClient;

        public PasswordResetController(IAccountClient accountClient)
        {
            _accountClient = accountClient;
        }

        [HttpGet("", Name = nameof(ResetPasswordGet))]
        public IActionResult ResetPasswordGet([FromQuery] string loginUrl)
        {
            if (loginUrl != null)
            {
                TempData["LoginUrl"] = loginUrl;
            }
            
            return View();
        }

        [HttpPost("", Name = nameof(ResetPasswordPost))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordPost([FromForm, Required] string userName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _accountClient.ResetPasswordAsync(userName, TempData["LoginUrl"] as string);
                return RedirectToAction(nameof(ResetPasswordConfirmationGet));
            }
            catch (ApiException e)
            {
                TempData["Errors"] = e.Errors;
            }

            return RedirectToAction(nameof(ResetPasswordGet));
        }

        [HttpGet("confirmation", Name = nameof(ResetPasswordConfirmationGet))]
        public IActionResult ResetPasswordConfirmationGet()
        {
            ViewData["LoginUrl"] = TempData["LoginUrl"];
            return View();
        }

        [HttpGet("complete", Name = nameof(CompletePasswordResetGet))]
        public IActionResult CompletePasswordResetGet([FromQuery] PasswordResetCompletionGetDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (dto.LoginUrl != null)
            {
                TempData["LoginUrl"] = dto.LoginUrl;
            }

            TempData["UserName"] = dto.UserName;
            TempData["Token"] = dto.Token;

            return View();
        }

        [HttpPost("complete", Name = nameof(CompletePasswordResetPost))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletePasswordResetPost(PasswordResetCompletionPostDto dto)
        {
            if (!ModelState.IsValid 
                || !TempData.TryGetValue("UserName", out var userName) 
                || !TempData.TryGetValue("Token", out var token))
            {
                return BadRequest();
            }
            TempData.Keep();

            try
            {
                await _accountClient.CompletePasswordResetAsync(userName as string, token as string, dto.Password);
                return RedirectToAction(nameof(CompletePasswordResetConfirmationGet));
            }
            catch (ApiException e)
            {
                TempData["Errors"] = e.Errors;
            }
            return RedirectToAction(nameof(CompletePasswordResetGet));
        }

        [HttpGet("complete/confirmation", Name = nameof(CompletePasswordResetConfirmationGet))]
        public IActionResult CompletePasswordResetConfirmationGet(string next)
        {
            ViewData["LoginUrl"] = TempData["LoginUrl"];
            TempData.Clear();
            return View();
        }
    }
}