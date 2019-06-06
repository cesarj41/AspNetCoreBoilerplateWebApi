using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using Web.ViewModels;
using Web.Extensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Infrastructure.Identity;

namespace Web.Controllers
{
    
    [Route("api/accounts")]
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("claims")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<string>> GetClaims()
        {
            return Ok(User.Claims
                .Where(claim => claim.Type == "auth")
                .Select(claim => claim.Value)
            );
        }

        [HttpGet("{userId}/confirm/{code}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new ErrorDetails(
                    $"Invalid user Id: '{userId}'."
                ));
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (! result.Succeeded)
            {
                return BadRequest(
                    new ErrorDetails(result.Errors())
                );
            }

            return NoContent();
        }

        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpGet("{email}/[action]")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ForgotPassword([FromRoute] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                string code = await  _userManager
                    .GeneratePasswordResetTokenAsync(user);

                // send url to user email with code
            }

            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApplicationUserViewModel>> RegisterUserAccount(
            RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(new ErrorDetails(
                    "user already has an account"
                ));
            }

            var user = new ApplicationUser(
                model.FirstName,
                model.LastName,
                model.Email
            );

    
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(
                    new ErrorDetails(result.Errors())
                );
            }
            var claimResult = await _userManager.AddClaimAsync(
                user, 
                new Claim("auth", "user")
            );

            if (!result.Succeeded)
            {
                throw new ApplicationException(
                    $"Adding claims errors: {result.Errors.ToString()}"
                );
            }
            
            return Ok(user.ToViewModel());
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(new ErrorDetails(
                    "user already logged in"
                ));
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                true,
                false
            );

            if (result.IsLockedOut)
            {
                return StatusCode(StatusCodes.Status423Locked);
            }

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorDetails(
                    "Invalid username or password"
                ));
            }

            return NoContent();
        }

        [HttpPost("{email}/[action]")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ResetPassword(
            [FromRoute] string email, 
            [FromBody] ResetAccountPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(
                model.Email
            );

            if (user == null)
            {
                return BadRequest(new ErrorDetails(
                    "Invalid email address"
                ));
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Code,
                model.Password
            );

            if (!result.Succeeded)
            {
                return BadRequest(
                    new ErrorDetails(result.Errors())
                );
            }

            return NoContent();        
        }

        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> ChangePassword(
            ChangeAccountPasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                return BadRequest(
                    new ErrorDetails(result.Errors())
                );
            }

            return NoContent();
        }

    }
}