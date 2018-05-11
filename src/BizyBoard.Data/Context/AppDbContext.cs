namespace BizyBoard.Data.Context
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models.DbEntities;

    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public int CurrentUserId { get; set; }
        public string CurrentUserFullName { get; set; }
        public int CurrentTenantId { get; set; }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<AppUserPhoto> AppUserPhotos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Tenant>().HasQueryFilter(t => !t.IsDeleted && t.Id == CurrentTenantId);
            builder.Entity<AppUserPhoto>().HasQueryFilter(t => !t.IsDeleted && t.TenantId == CurrentTenantId);

            builder.Entity<Tenant>().Property(t => t.RowVersion).IsConcurrencyToken();
            builder.Entity<AppUserPhoto>().Property(t => t.RowVersion).IsConcurrencyToken();
            builder.Entity<AppUser>().HasOne(e => e.Tenant).WithMany(e => e.Users).HasForeignKey(e => e.TenantId);

            builder.Entity<Tenant>().HasOne(t => t.CreatedBy);
            builder.Entity<AppUserPhoto>().HasOne(t => t.CreatedBy);

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            UpdateAuditEntities();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateAuditEntities()
        {
            var modifiedEntries = ChangeTracker.Entries().Where(x => x.Entity is IEntityBase && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                var entity = (IEntityBase)entry.Entity;
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreationDate = now;
                    entity.CreatedById = CurrentUserId;
                    entity.CreatedByFullName = CurrentUserFullName;
                }
                else
                {
                    Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    Entry(entity).Property(x => x.CreatedById).IsModified = false;
                }

                entity.LastUpdateDate = now;
                entity.LastUpdateById = CurrentUserId;
                entity.LastUpdateByFullName = CurrentUserFullName;
            }
        }
    }
}