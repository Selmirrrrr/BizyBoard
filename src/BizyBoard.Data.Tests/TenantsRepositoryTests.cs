namespace BizyBoard.Data.Tests
{
    using System;
    using Context;
    using Microsoft.EntityFrameworkCore;
    using Models.DbEntities;
    using Repositories;
    using Xunit;

    public class TenantsRepositoryTests
    {
        private readonly IRepository<Tenant> _tenantsRepository;
        private readonly IRepository<Tenant> _tenantsRepositoryDuplicata;
        public TenantsRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(nameof(TenantsRepositoryTests))
                .Options;

            _tenantsRepository = new Repository<Tenant>(new AppDbContext(options));

            var db = new AppDbContext(options) { CurrentTenantId = 1 };
            _tenantsRepositoryDuplicata = new Repository<Tenant>(db);
        }

        [Fact]
        public void Add_ShouldInserTenantsToDb_WhenNameIsSetted()
        {
            _tenantsRepository.Add(new Tenant { Name = "Test", Id = 1 });
            _tenantsRepository.Commit();

            var tenant = _tenantsRepositoryDuplicata.GetSingle(t => t.Name == "Test");

            Assert.NotNull(tenant);
        }
    }
}
