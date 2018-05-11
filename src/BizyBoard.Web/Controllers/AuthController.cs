namespace BizyBoard.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Repositories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Models.Core;
    using Models.DbEntities;
    using Models.ViewModels;
    using Services;

    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailService _emailService;
        private readonly ITenantsRepository _tenantRepo;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<AppUser> userManager, 
                                 SignInManager<AppUser> signInManager,
                                 IConfiguration configuration,
                                 IEmailService emailService,
                                 ITenantsRepository tenantRepo,
                                 ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _signInManager = signInManager;
            _emailService = emailService;
            _tenantRepo = tenantRepo;
            _userManager = userManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model, string returnUrl = null)
        {
            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                Firstname = model.Lastname,
                Lastname = model.Lastname,
            };

            var tenant = new Tenant
            {
                Name = model.Company
            };

            _tenantRepo.Add(tenant, user);

            user.Tenant = tenant;

            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Add to roles
                    var roleAddResult = await _userManager.AddToRoleAsync(user, "PowerUser");
                    if (roleAddResult.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var host = Request.Scheme + "://" + Request.Host;
                        var callbackUrl = host + "?userId=" + user.Id + "&emailConfirmCode=" + code;
                        var confirmationLink = "<a class='btn-primary' href=\"" + callbackUrl + "\">Confirm email address</a>";
                        _logger.LogInformation(3, "User created a new account with password.");
                        await _emailService.SendEmailAsync(model.Email, "Registration confirmation email", confirmationLink);
                        return SignIn(user, new List<string>{"PowerUser"});
                    }
                }
                AddErrors(result);
            }
            catch (Exception ex)
            {
                var test = ex;
            }
            
            // If we got this far, something failed, redisplay form
            return BadRequest(new BaseError(ModelState));
        }

        // POST: /Account/Login
        [HttpPost("login")]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation(1, "User logged in.");
                return SignIn(user, roles);
            }
            if (result.RequiresTwoFactor)
                return RedirectToAction("SendCode", new { model.RememberMe });
            if (!result.IsLockedOut) return BadRequest(new BaseError("Invalid login attempt."));
            _logger.LogWarning(2, "User account locked out.");
            return BadRequest(new BaseError("Lockout"));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return NoContent();
        }

        [HttpGet("SendCode")]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return BadRequest(new BaseError("Error"));

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { 
                Text = purpose, 
                Value = purpose 
            }).ToList();
            return Ok(new SendCodeViewModel { Providers = factorOptions, 
                                              ReturnUrl = returnUrl, 
                                              RememberMe = rememberMe });
        }

        private string GenerateJwtToken(string email, AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),

                Expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        private IActionResult SignIn(AppUser user, IList<string> roles)
        {
            var userResult = new 
            { 
                User = new 
                { 
                    DisplayName = user.UserName, 
                    Roles = roles,
                    user.Id
                }, 
                Token = GenerateJwtToken(user.Id.ToString(), user)
            };

            return new ObjectResult(userResult);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}