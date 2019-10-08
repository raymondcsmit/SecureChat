using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using Registration.Exceptions;
using Registration.Models;
using Registration.Services;

namespace Registration.Controllers
{
    [Route("email-confirmation")]
    public class EmailConfirmationController : Controller
    {
        private readonly IUsersClient _usersClient;

        public EmailConfirmationController(IUsersClient usersClient)
        {
            _usersClient = usersClient;
        }

        [HttpGet("", Name = nameof(ConfirmEmailGet))]
        public async Task<IActionResult> ConfirmEmailGet(EmailConfirmationDto emailConfirmationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _usersClient.ConfirmEmailAsync(emailConfirmationDto);
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