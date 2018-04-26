namespace Bizy.WinBizApi.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Services;
    using Web.Services;
    using Xunit;
    using static System.Environment;

    public class WinBizApiServiceUnitTests
    {
        private readonly WinBizApiService _service;

        public WinBizApiServiceUnitTests()
        {
            var settingsService = new SettingsService(new WinBizApiSettings(GetEnvironmentVariable("WINBIZ_API_KEY"),
                GetEnvironmentVariable("WINBIZ_API_COMPANY"),
                GetEnvironmentVariable("WINBIZ_API_USERNAME"),
                GetEnvironmentVariable("WINBIZ_API_PASSWORD"),
                "BgIAAACkAABSU0ExAAQAAAEAAQBZ3myd6ZQA0tUXZ3gIzu1sQ7larRfM5KFiYbkgWk+jw2VEWpxpNNfDw8M3MIIbbDeUG02y/ZW+XFqyMA/87kiGt9eqd9Q2q3rRgl3nWoVfDnRAPR4oENfdXiq5oLW3VmSKtcBl2KzBCi/J6bbaKmtoLlnvYMfDWzkE3O1mZrouzA==",
                "https://api.winbizcloud.ch/"));
            _service = new WinBizApiService(settingsService.WinBizApiSettings, 1, 2017);
        }

        [Fact]
        public void GetAdId_ReturnsAdId_WhenAdCodeExists()
        {
            //to be implemented
        }

        [Fact]
        public async Task GetStock_ReturnsStock_WhenProductExists()
        {
            var response = await _service.GetStock(2).ConfigureAwait(false);

            Assert.True(int.TryParse(response, out var stock));
            Assert.True(stock == 111);
        }

        [Fact]
        public async Task Adresses_ReturnsAddressesList_WhenNoLimitDateIsSet()
        {
            var response = await _service.Adresses().ConfigureAwait(false);

            Assert.True(response.Any());
        }
    }
}
