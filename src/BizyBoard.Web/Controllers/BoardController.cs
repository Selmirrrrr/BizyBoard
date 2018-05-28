﻿namespace BizyBoard.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Auth;
    using Bizy.OuinneBiseSharp.Enums;
    using Bizy.OuinneBiseSharp.Services;
    using Core.Helpers;
    using Core.Permissions;
    using Data.Context;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.DbEntities;
    using Microsoft.AspNetCore.Authorization;

    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.CanSeeDashboard)]
    [Authorize]
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
        [Route("{nbMonths}")]
        public async Task<IActionResult> GetDocInfoVenteChiffreAffaire(int nbMonths)
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
                    .Select(o => new { Label = o.Result.Month, o.Result.Result.Value });
                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(nameof(GetDocInfoVenteChiffreAffaire), e);
                return new BadRequestObjectResult(new { code = e.Message, message = e.ToString() });
            }
        }

        private async Task<AppUser> GetUser() 
            => await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Id, StringComparison.InvariantCultureIgnoreCase))?.Value);

        private string GetCompany() 
            => User.Claims.FirstOrDefault(c => string.Equals(c.Type, Constants.Strings.JwtClaimIdentifiers.Company, StringComparison.InvariantCultureIgnoreCase))?.Value;
    }
}