using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using Registration.Exceptions;
using Registration.Models;
using Registration.Services;
using Registration.ViewModels;

namespace Registration.Controllers
{
    [Route("password-reset")]
    public class PasswordResetController : Controller
    {
        private readonly IUsersClient _usersClient;
        private readonly ILoginUrlService _loginUrlService;

        public PasswordResetController(IUsersClient usersClient, ILoginUrlService loginUrlService)
        {
            _usersClient = usersClient;
            _loginUrlService = loginUrlService;
        }

        [HttpGet("", Name = nameof(ResetPasswordGet))]
        public IActionResult ResetPasswordGet(string loginUrl)
        {
            ViewData["Errors"] = TempData["Errors"];
            if (loginUrl != null)
            {
                _loginUrlService.SetLoginUrlCookie(loginUrl);
            }
            
            return View();
        }

        [HttpPost("", Name = nameof(ResetPasswordPost))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordPost(PasswordResetDto passwordResetDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _usersClient.ResetPasswordAsync(passwordResetDto);
                return RedirectToAction(nameof(ResetPasswordConfirmationGet));
            }
            catch (ApiException e)
            {
                return RedirectToAction(nameof(ResetPasswordGet), new
                {
                    e.Errors
                });
            }
        }

        [HttpGet("confirmation", Name = nameof(ResetPasswordConfirmationGet))]
        public IActionResult ResetPasswordConfirmationGet()
        {
            ViewData["LoginUrl"] = _loginUrlService.GetLoginUrl();
            _loginUrlService.ClearLoginUrlCookie();
            return View();
        }

        [HttpGet("complete", Name = nameof(CompletePasswordResetGet))]
        public IActionResult CompletePasswordResetGet(PasswordResetCompletionGetDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (dto.LoginUrl != null)
            {
                _loginUrlService.SetLoginUrlCookie(dto.LoginUrl);
            }

            return View(new PasswordResetViewModel()
            {
                UserName = dto.UserName,
                Token = dto.Token
            });
        }

        [HttpPost("complete", Name = nameof(CompletePasswordResetPost))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletePasswordResetPost(PasswordResetCompletionPostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _usersClient.CompletePasswordResetAsync(dto);
                return RedirectToAction(nameof(CompletePasswordResetConfirmationGet));
            }
            catch (TaskCanceledException)
            {
                return RedirectToAction(nameof(CompletePasswordResetGet), new PasswordResetViewModel()
                {
                    UserName = dto.UserName,
                    Token = dto.Token
                });
            }
        }

        [HttpGet("complete/confirmation", Name = nameof(CompletePasswordResetConfirmationGet))]
        public IActionResult CompletePasswordResetConfirmationGet(string next)
        {
            ViewData["LoginUrl"] = _loginUrlService.GetLoginUrl();
            _loginUrlService.ClearLoginUrlCookie();
            return View();
        }
    }
}