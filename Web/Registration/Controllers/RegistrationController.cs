using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using Registration.Exceptions;
using Registration.Models;
using Registration.Services;
using Registration.Views.Registration;

namespace Registration.Controllers
{
    [Route("registration")]
    public class RegistrationController : Controller
    {
        private readonly IUsersClient _usersClient;
        private readonly ILoginUrlService _loginUrlService;

        public RegistrationController(IUsersClient usersClient, ILoginUrlService loginUrlService)
        {
            _usersClient = usersClient;
            _loginUrlService = loginUrlService;
        }

        [HttpGet("", Name = nameof(RegisterGet))]
        public IActionResult RegisterGet([FromQuery] string loginUrl)
        {
            ViewData["Errors"] = TempData["Errors"];
            if (loginUrl != null)
            {
                _loginUrlService.SetLoginUrlCookie(loginUrl);
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
                return RedirectToAction(nameof(RegisterConfirmationGet), user);
            }
            catch (ApiException e)
            {
                TempData["Errors"] = e.Errors;
            }

            return RedirectToAction(nameof(RegisterGet));

        }

        [HttpGet("confirmation", Name = nameof(RegisterConfirmationGet))]
        public IActionResult RegisterConfirmationGet(User user)
        {
            ViewData["LoginUrl"] = _loginUrlService.GetLoginUrl();
            _loginUrlService.ClearLoginUrlCookie();
            return View(user);
        }
    }
}