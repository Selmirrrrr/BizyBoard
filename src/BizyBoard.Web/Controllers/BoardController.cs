namespace BizyBoard.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Bizy.OuinneBiseSharp.Enums;
    using Bizy.OuinneBiseSharp.Services;
    using Core.Helpers;
    using Data.Repositories;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.DbEntities;

    [Route("api/[controller]")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class BoardController : Controller
    {
        private readonly IRepository<Tenant> _tenantsRepository;
        private readonly OuinneBiseSharpService _service;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<BoardController> _logger;
        private readonly AppUser _user;

        public BoardController(IOuinneBiseSharpFactory factory, UserManager<AppUser> userManager, ILogger<BoardController> logger)
        {
            _service = factory.GetInstance("sdasd", "fdsdfs");
            //_tenantsRepository = tenantsRepository;
            _userManager = userManager;
            _logger = logger;
        }


        public async Task<IActionResult> GetDocInfoVenteChiffreAffaire()
        {
            int.TryParse(User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Id, StringComparison.InvariantCultureIgnoreCase))?.Value, out var userId);

            var user = _userManager.FindByIdAsync(userId.ToString());

            if (user == null) return BadRequest("Tenant introuvable dans la base de données.");

            var result = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.MinValue, DateTime.MaxValue);

            return Ok(result);
        }
    }
}