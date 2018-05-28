namespace BizyBoard.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using Auth;
    using AutoMapper;
    using Bizy.OuinneBiseSharp.Extensions;
    using Core.Permissions;
    using Data.Context;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models.DbEntities;
    using Models.ViewModels;
    using Services;
    using static Core.Helpers.Constants.Strings;

    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AdminDbContext _dbContext;
        private readonly IJwtFactory _jwtFactory;
        private readonly ILogger<AuthController> _logger;
        private readonly IOuinneBiseSharpFactory _factory;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<AppUser> userManager,
                                 AdminDbContext dbContext,
                                 IJwtFactory jwtFactory,
                                 IOptions<JwtIssuerOptions> jwtOptions,
                                 IOuinneBiseSharpFactory factory,
                                 IEmailService emailService,
                                 IMapper mapper,
                                 ILogger<AuthController> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _dbContext = dbContext;
            _jwtFactory = jwtFactory;
            _factory = factory;
            _emailService = emailService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == model.Email))
                return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Errors.DuplicateEmail, ModelState));

            var userIdentity = _mapper.Map<AppUser>(model);

            var tenant = new Tenant
            {
                Name = model.Company,
                CreatedByFullName = userIdentity.FullName,
                LastUpdateByFullName = userIdentity.FullName,
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            };

            tenant = _dbContext.Tenants.Add(tenant).Entity;
            await _dbContext.SaveChangesAsync();

            userIdentity.Tenant = tenant;

            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded)
            {
                _dbContext.Tenants.Remove(tenant);
                await _dbContext.SaveChangesAsync();
                return new BadRequestObjectResult(ErrorsHelper.AddErrorsToModelState(result, ModelState));
            }

            var user = await _userManager.FindByEmailAsync(userIdentity.Email);

            await _userManager.AddToRoleAsync(user, Roles.TenantAdmin);

            tenant.CreatedBy = user;
            tenant.LastUpdateBy = user;

            _dbContext.Tenants.Update(tenant);

            await _dbContext.SaveChangesAsync();

            return new OkObjectResult(await Tokens.GenerateJwt(await GetClaimsIdentity(userIdentity.Email, model.Password), _jwtFactory, userIdentity.Email, _jwtOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var identity = await GetClaimsIdentity(credentials.Email, credentials.Password);
            if (identity == null) return BadRequest(ErrorsHelper.AddErrorToModelState(Errors.LoginFailure, ModelState));

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Email, _jwtOptions);

            return new OkObjectResult(jwt);
        }
        
        [HttpPost]
        public async Task<IActionResult> TestWinBizCredentials([FromBody] RegistrationViewModel credentials)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var service = _factory.GetInstance(credentials.Company, credentials.WinBizUsername, credentials.WinBizPassword.Encrypt());

            try
            {
                var folders = await service.Folders();
                if (folders.ErrorsCount > 0) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState("winbiz_error", folders.UserErrorMsg, ModelState));
                if (folders.Value.Count < 1) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Errors.NoWinBizFolder, ModelState));

                return new OkObjectResult(folders.Value.Select(d => new { d.Number, d.Name, Exercices = d.Exercices.Select(e => new { e.Year, e.Start, e.End, e.Description, e.IsClosed, Dossier = d.Number }) }).ToList());
            }
            catch (Exception e)
            {
                credentials.CleanPasswords();
                _logger.LogCritical(e, nameof(TestWinBizCredentials), credentials);
                return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Errors.Base, ModelState));
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]  
        public async Task<IActionResult> ForgotPassword([FromBody]ResetPwdViewModel vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null) return new OkObjectResult(Success.PasswordReset);

            if (!await _userManager.IsEmailConfirmedAsync(user)) return new OkObjectResult(Success.PasswordReset);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}/#/updatepwd";

            await _emailService.SendEmailAsync(user.Email, "Demande de réinitialisation de mot de passe", baseUrl + "?token=" + HttpUtility.HtmlEncode(token) + "&email=" + user.Email);

            return new OkObjectResult(Success.PasswordReset);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword([FromBody]ResetPwdUpdateViewModel vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(vm.Email);

            var result = await _userManager.ResetPasswordAsync(user, HttpUtility.HtmlDecode(vm.Token)?.Replace(" ", "+"), vm.NewPassword);
            if (result.Succeeded) return new OkObjectResult(Success.PasswordResetUpdate);

            return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Errors.PasswordResetUpdateError, ModelState));
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (!await _userManager.CheckPasswordAsync(userToVerify, password)) return await Task.FromResult<ClaimsIdentity>(null);
            var roles = await _userManager.GetRolesAsync(userToVerify);

            var company = await _dbContext.Tenants.Where(t => t.Id == userToVerify.TenantId).Select(t => new Tenant { Name = t.Name }).FirstOrDefaultAsync();


            return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id, company.Name, userToVerify.TenantId, roles));
        }
    }
}