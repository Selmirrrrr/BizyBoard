namespace BizyBoard.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Auth;
    using Bizy.OuinneBiseSharp.Enums;
    using Bizy.OuinneBiseSharp.Services;
    using Core.Helpers;
    using Data.Context;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.DbEntities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.CanSeeDashboard)]
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class BoardController : Controller
    {
        private OuinneBiseSharpService _service;
        private readonly AppDbContext _context;
        private readonly IOuinneBiseSharpFactory _factory;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<BoardController> _logger;

        public BoardController(AppDbContext context, IOuinneBiseSharpFactory factory, UserManager<AppUser> userManager, ILogger<BoardController> logger)
        {
            _context = context;
            _factory = factory;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDocInfoVenteChiffreAffaireThisYear()
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);
                var result = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, new DateTime(DateTime.Now.Year, 12, 31), new DateTime(DateTime.Now.Year, 1, 1));

                return Ok(result.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                return new BadRequestObjectResult(Constants.Strings.Errors.Base);
            }
        }

        [HttpGet]
        [Route("{nbMonths}")]
        public async Task<IActionResult> GetDocInfoVenteChiffreAffaireMonths([BindRequired, FromRoute] int nbMonths)
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);
                var results = Enumerable
                    .Range(0, nbMonths)
                    .Select(i => DateTime.Now.AddMonths(i - nbMonths).AddDays(DateTime.Now.Day - 1))
                    .Select(async d => new { Result = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, d.AddDays(30), d), Month = d.ToString("MMM") })
                    .Select(o => new { Label = o.Result.Month, o.Result.Result.Value }).ToList();

                _logger.LogInformation("Results", results);
                return Ok(results);
            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                return new BadRequestObjectResult(Constants.Strings.Errors.Base);
            }
        }

        [HttpGet]
        [Route("{nbYears}")]
        public async Task<IActionResult> GetDocInfoVenteChiffreAffaireYears([BindRequired, FromRoute] int nbYears)
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);
                var results = Enumerable
                    .Range(0, nbYears)
                    .Select(i => new DateTime(DateTime.Now.AddYears(i - nbYears).Year, 1, 1))
                    .Select(async d => new { Result = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, d.AddMonths(12).AddDays(30), d), d.Year })
                    .Select(o => new { Label = o.Result.Year, o.Result.Result.Value }).ToList();

                _logger.LogInformation("Results", results);
                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                return new BadRequestObjectResult(Constants.Strings.Errors.Base);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);
                var folders = await _service.Folders();
                if (folders.ErrorsCount > 0) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState("winbiz_error", folders.UserErrorMsg, ModelState));
                if (folders.Value.Count < 1) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.NoWinBizFolder, ModelState));

                return new OkObjectResult(folders.Value.Select(d => new { d.Number, d.Name, Exercice = d.Exercices.OrderBy(e => e.Year).LastOrDefault()?.Year ?? 2018 }).ToList());
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, nameof(GetFolders));
                return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.Base, ModelState));
            }
        }

        private async Task<AppUser> GetUser() 
            => await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Id, StringComparison.InvariantCultureIgnoreCase))?.Value);

        private string GetCompany() 
            => User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Company, StringComparison.InvariantCultureIgnoreCase))?.Value;
    }
}