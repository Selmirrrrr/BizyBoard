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
                    .Select(async d => new
                    {
                        Result = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, d.AddMonths(12).AddDays(30), d), 
                        ResultToDate = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, d.AddMonths(DateTime.Now.Month).AddDays(DateTime.Now.Day), d), 
                        d.Year
                    })
                    .Select(o => new { Label = o.Result.Year, Year = o.Result.Result.Value, YearToDate = o.Result.ResultToDate }).ToList();

                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                //return new BadRequestObjectResult(Constants.Strings.Errors.Base);
                return Ok(new[]
                {
                    new {Label = 2018, YearToDate = 6000, Year = 6000},
                    new {Label = 2017, YearToDate = 11000, Year = 24000},
                    new {Label = 2016, YearToDate = 10000, Year = 20000},
                    new {Label = 2015, YearToDate = 8000, Year = 15000}
                });
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

        [HttpGet]
        public async Task<IActionResult> GetSalesThisAndPastYear()
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);

                var salesThisYear = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.Now, new DateTime(DateTime.Now.Year, 1, 1));
                var salesPastYear = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.Now.AddYears(-1), new DateTime(DateTime.Now.Year, 1, 1).AddYears(-1));

                return Ok(new { salesThisYear, salesPastYear });
            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                //return new BadRequestObjectResult(Constants.Strings.Errors.Base);
                return Ok(new { salesThisYear = 1233, salesPastYear = 23324 });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesThisAndPastYearMonth()
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);

                var salesThisMonth = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.Now, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                var salesPastYearMonth = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire,
                    new DateTime(DateTime.Now.AddYears(-1).Year, DateTime.Now.Month, 31),
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddYears(-1));

                return Ok(new { salesThisMonth, salesPastYearMonth });
            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                //return new BadRequestObjectResult(Constants.Strings.Errors.Base);
                return Ok(new { salesThisMonth = 1233, salesPastYearMonth = 23324 });

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingPayments()
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);

                var res = await _service.PendingPayments(9999);

                return Ok(new { Amount = res.Value.Sum(r => r.LocalOpenAmount), Docs = res.Value.Sum(r => r.OpenDocuments) });
            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                //return new BadRequestObjectResult(Constants.Strings.Errors.Base);
                return Ok(new { Amount = 12, Docs = 1234 });

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentsCalendar()
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);

                var res = await _service.PaymentsCalendar(9999);

                return Ok(new { Amount = res.Value.Sum(r => r.LocalOpenAmount), Docs = res.Value.Sum(r => r.OpenDocuments) });
            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                //return new BadRequestObjectResult(Constants.Strings.Errors.Base);
                return Ok(new { Amount = 23, Docs = 3452 });

            }
        }

        [HttpGet]
        [Route("{nb}")]
        public async Task<IActionResult> GetBadPayersList(int nb)
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);

                var res = await _service.AddressesPendingPayments(9999);
                var values = res.Value.GroupBy(r => r.AddressId)
                                      .OrderByDescending(r => r.Sum(o => o.LocalOpenAmount))
                                      .Select(r => new {r.FirstOrDefault()?.Address, Amount = r.Sum(o => o.LocalOpenAmount), Count = r.Sum(o => o.OpenDocuments)}).Take(nb == 0 ? 99999 : nb);
                return Ok(values);
            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                //return new BadRequestObjectResult(Constants.Strings.Errors.Base);
                return Ok(new[]
                {
                    new { Address = "Lol", Amount = 100000, Count = 12},
                    new { Address = "Lol2", Amount = 200000, Count = 22},
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var user = await GetUser();
            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));
            var company = GetCompany();
            if (string.IsNullOrWhiteSpace(company)) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.CompanyNotFound, ModelState));

            try
            {
                _service = _factory.GetInstance(company, user);

                var res = await _service.Addresses();
                return Ok(res.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaireMonths), e);
                return new BadRequestObjectResult(Constants.Strings.Errors.Base);
            }
        }

        private async Task<AppUser> GetUser()
            => await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Id, StringComparison.InvariantCultureIgnoreCase))?.Value);

        private string GetCompany()
            => User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Company, StringComparison.InvariantCultureIgnoreCase))?.Value;
    }
}