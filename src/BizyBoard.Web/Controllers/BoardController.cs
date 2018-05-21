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
    using Microsoft.EntityFrameworkCore;

    [Authorize(Policy = "Admin")]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetDocInfoVenteChiffreAffaire()
        {
            int.TryParse(User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Id, StringComparison.InvariantCultureIgnoreCase))?.Value, out var userId);
            var company = User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Company, StringComparison.InvariantCultureIgnoreCase))?.Value;

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) return new BadRequestObjectResult(ErrorsHelper.AddErrorToModelState(Constants.Strings.Errors.UserNotFound, ModelState));

            try
            {

                _service = _factory.GetInstance(company, user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                var result = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.MaxValue, DateTime.MinValue);
                return Ok(result);

            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaire), e);
                return new BadRequestObjectResult(Constants.Strings.Errors.Base);
            }

        }
    }
}