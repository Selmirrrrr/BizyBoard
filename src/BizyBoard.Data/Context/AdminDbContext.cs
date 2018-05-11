namespace BizyBoard.Data.Context
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models.DbEntities;

    public class AdminDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public DbSet<Tenant> Tenants { get; set; }

        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Tenant>().HasQueryFilter(t => !t.IsDeleted);
            builder.Entity<AppUserPhoto>().HasQueryFilter(t => !t.IsDeleted);

            builder.Entity<Tenant>().Property(t => t.RowVersion).IsConcurrencyToken();
            builder.Entity<AppUser>().HasOne(e => e.Tenant).WithMany(e => e.Users).HasForeignKey(e => e.TenantId);

            builder.Entity<Tenant>().HasOne(t => t.CreatedBy);
            builder.Entity<AppUserPhoto>().HasOne(t => t.CreatedBy);

            base.OnModelCreating(builder);
        }
    }
}