namespace BizyBoard.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Auth;
    using AutoMapper;
    using Bizy.OuinneBiseSharp.Extensions;
    using Core.Helpers;
    using Core.Services;
    using Data.Context;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models.DbEntities;
    using Models.ViewModels;

    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AdminDbContext _dbContext;
        private readonly IJwtFactory _jwtFactory;
        private readonly ILogger<AuthController> _logger;
        private readonly RolesService _rolesService;
        private readonly IOuinneBiseSharpFactory _factory;
        private readonly IMapper _mapper;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<AppUser> userManager,
                                 AdminDbContext dbContext,
                                 IJwtFactory jwtFactory,
                                 IOptions<JwtIssuerOptions> jwtOptions,
                                 RolesService rolesService,
                                 IOuinneBiseSharpFactory factory,
                                 IMapper mapper,
                                 ILogger<AuthController> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _dbContext = dbContext;
            _jwtFactory = jwtFactory;
            _rolesService = rolesService;
            _factory = factory;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_userManager.Users.Any(u => u.NormalizedEmail == model.Email))
                return new BadRequestObjectResult(Errors.AddErrorToModelState("account_creation_failure", "Email déjà existant.", ModelState));

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
            _dbContext.SaveChanges();

            userIdentity.Tenant = tenant;

            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded)
            {
                _dbContext.Tenants.Remove(tenant);
                await _dbContext.SaveChangesAsync();
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
            }

            var user = await _userManager.FindByEmailAsync(userIdentity.Email);

            await _userManager.AddToRoleAsync(user, _rolesService.TenantAdmin);

            tenant.CreatedBy = user;
            tenant.LastUpdateBy = user;

            _dbContext.Tenants.Update(tenant);

            _dbContext.SaveChanges();

            await _dbContext.SaveChangesAsync();

            return new OkObjectResult(await Tokens.GenerateJwt(await GetClaimsIdentity(userIdentity.Email, model.Password), _jwtFactory, userIdentity.Email, _jwtOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var identity = await GetClaimsIdentity(credentials.Email, credentials.Password);
            if (identity == null) return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Email, _jwtOptions);

            return new OkObjectResult(jwt);
        }

        [HttpPost]
        public async Task<IActionResult> TestWinBizCredentials([FromBody] RegistrationViewModel credentials)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var service = _factory.GetInstance(credentials.Company, credentials.WinBizUsername, credentials.WinBizPassword.Encrypt(Constants.Strings.WinBizEncryptionKey));

            try
            {
                var folders = await service.Folders();
                if (folders.ErrorsCount > 0) return new BadRequestObjectResult(folders.UserErrorMsg);
                if (folders.Value.Count < 1) return new BadRequestObjectResult("Pas de dossier ouvert dans WinBIZ Cloud");

                return new OkObjectResult(folders.Value.Select(d => new { d.Number, d.Name, Exercices = d.Exercices.Select(e => new { e.Year, e.Start, e.End, e.Description, e.IsClosed, Dossier = d.Number }) }).ToList());
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }

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
            return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id, userToVerify.TenantId, roles));

            // Credentials are invalid, or account doesn't exist
        }
    }
}