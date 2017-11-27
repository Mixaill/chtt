using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using chtt.Models;
using chtt.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace chtt.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private IConfiguration _configuration;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        /// <summary>
        /// Logins into existing account
        /// </summary>
        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public async Task<IActionResult> LoginAccount([FromBody] LoginViewModel userInput)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(userInput.Email, userInput.Password, true, false);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = new JwtSecurityToken
            (
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userInput.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                expires: DateTime.UtcNow.AddDays(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecurityKey"])), SecurityAlgorithms.HmacSha256)
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        /// <summary>
        /// Log outs from account
        /// </summary>
        [HttpPost("Logout")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> LogoutAccount()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        /// <summary>
        /// Registers a new user account
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public async Task<IActionResult> RegisterAccount([FromBody] RegisterViewModel userInput)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = userInput.Email, Email = userInput.Email };
                var result = await _userManager.CreateAsync(user,userInput.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}