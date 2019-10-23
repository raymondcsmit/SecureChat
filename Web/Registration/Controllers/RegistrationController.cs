using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public RegistrationController(IUsersClient usersClient, ILoginUrlService loginUrlService, IConfiguration configuration)
        {
            _usersClient = usersClient;
            _loginUrlService = loginUrlService;
            _configuration = configuration;
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
            ViewData["LoginUrl"] = _loginUrlService.GetLoginUrl() ?? $"{_configuration["AuthUrl"]}/account/login";
            _loginUrlService.ClearLoginUrlCookie();
            return View();
        }
    }
}