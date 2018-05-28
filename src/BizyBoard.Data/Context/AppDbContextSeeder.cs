namespace BizyBoard.Data.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Bizy.OuinneBiseSharp.Extensions;
    using Core.Permissions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Models.DbEntities;

    public class AppDbContextSeeder
    {
        private readonly AppDbContext _context;
        private readonly AdminDbContext _adminDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<AppDbContextSeeder> _logger;

        public AppDbContextSeeder(AppDbContext context,
            AdminDbContext adminDbContext,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            ILogger<AppDbContextSeeder> logger)
        {
            _context = context;
            _adminDbContext = adminDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task Migrate() => await _context.Database.MigrateAsync();

        public async Task Seed()
        {
            if (_context.Roles.Any()) return;

            var rolesToAdd = new List<(AppRole role, string permission)> {
                (new AppRole { Name = Roles.Admin, Description = "Full rights role" }, Policies.CanSeeDashboard),
                (new AppRole { Name = Roles.TenantAdmin, Description = "Tenant admin role"}, Policies.CanSeeDashboard),
                (new AppRole { Name = Roles.TenantUser, Description = "Tenant user role"}, Policies.CanSeeDashboard)
            };

            foreach (var (role, permission) in rolesToAdd)
            {
                await _roleManager.CreateAsync(role);
                var userRole = await _roleManager.FindByNameAsync(role.Name);
                await _roleManager.AddClaimAsync(userRole, new Claim(CustomClaimTypes.Permission, permission));
            }

            var tenant = new Tenant
            {
                Name = "BizyDev",
                CreatedByFullName = "Selmir Hajruli",
                LastUpdateByFullName = "Selmir Hajruli",
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            };

            tenant = _adminDbContext.Tenants.Add(tenant).Entity;
            _adminDbContext.SaveChanges();


            await _userManager.CreateAsync(new AppUser
            {
                UserName = "info@bizy.ch",
                Firstname = "Selmir",
                Lastname = "Hajruli",
                Email = "info@bizy.ch",
                LastErpFiscalYear = 2018,
                LastErpCompanyId = 2,
                ErpUsername = Environment.GetEnvironmentVariable("WINBIZ_API_USERNAME"),
                ErpPassword = Environment.GetEnvironmentVariable("WINBIZ_API_PASSWORD").Encrypt(
                    "BgIAAACkAABSU0ExAAQAAAEAAQBZ3myd6ZQA0tUXZ3gIzu1sQ7larRfM5KFiYbkgWk+jw2VEWpxpNNfDw8M3MIIbbDeUG02y/ZW+XFqyMA/87kiGt9eqd9Q2q3rRgl3nWoVfDnRAPR4oENfdXiq5oLW3VmSKtcBl2KzBCi/J6bbaKmtoLlnvYMfDWzkE3O1mZrouzA=="),
                EmailConfirmed = true,
                
                Tenant = tenant
            }, Environment.GetEnvironmentVariable("USER_PASSWORD"));

            var user = await _userManager.FindByNameAsync("info@bizy.ch");

            await _userManager.AddToRoleAsync(user, Roles.Admin);

            tenant.CreatedBy = user;
            tenant.LastUpdateBy = user;

            _adminDbContext.Tenants.Update(tenant);

            _adminDbContext.SaveChanges();
        }
    }
}