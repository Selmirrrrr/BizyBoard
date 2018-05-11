namespace BizyBoard.Data.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Models.DbEntities;

    public class AppDbContextSeeder
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly RolesService _rolesService;
        private readonly ILogger<AppDbContextSeeder> _logger;

        public AppDbContextSeeder(AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            RolesService rolesService,
            ILogger<AppDbContextSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _rolesService = rolesService;
            _logger = logger;
        }

        public async Task Seed()
        {
            await _context.Database.MigrateAsync();

            if (_context.Roles.Any()) return;

            var rolesToAdd = new List<AppRole>(){
                new AppRole { Name = _rolesService.Admin, Description = "Full rights role" },
                new AppRole { Name = _rolesService.TenantAdmin, Description = "Tenant admin role"},
                new AppRole { Name = _rolesService.TenantUser, Description = "Tenant user role"}
            };

            foreach (var appRole in rolesToAdd) await _roleManager.CreateAsync(appRole);

            var tenant = new Tenant
            {
                Name = "Bizy",
                CreatedByFullName = "Selmir Hajruli",
                LastUpdateByFullName = "Selmir Hajruli",
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            };

            tenant = _context.Add(tenant).Entity;

            await _userManager.CreateAsync(new AppUser
            {
                UserName = "info@bizy.ch",
                Firstname = "Selmir",
                Lastname = "Hajruli",
                Email = "info@bizy.ch",
                EmailConfirmed = true,
                Tenant = tenant
            }, "P@ssw0rd!");

            var user = await _userManager.FindByNameAsync("admin@admin.com");

            await _userManager.AddToRoleAsync(user, _rolesService.Admin);

            tenant.CreatedBy = user;
            tenant.LastUpdateBy = user;

            _context.Tenants.Update(tenant);

            _context.SaveChanges();
        }
    }
}