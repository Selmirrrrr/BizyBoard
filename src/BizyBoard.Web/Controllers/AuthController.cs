namespace BizyBoard.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Auth;
    using AutoMapper;
    using Core.Services;
    using Data.Context;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<AppUser> userManager, 
                                 AdminDbContext dbContext,
                                 IJwtFactory jwtFactory,
                                 IOptions<JwtIssuerOptions> jwtOptions,
                                 RolesService rolesService,
                                 IMapper mapper,
                                 ILogger<AuthController> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _dbContext = dbContext;
            _jwtFactory = jwtFactory;
            _rolesService = rolesService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

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