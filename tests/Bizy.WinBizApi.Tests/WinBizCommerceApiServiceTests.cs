namespace Bizy.WinBizApi.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Enums;
    using Microsoft.Extensions.Logging;
    using Web.Services;
    using Xunit;
    using static System.Environment;
    using WinBizCommerceApiService = Services.WinBizCommerceApiService;

    public class WinBizCommerceApiServiceTests
    {
        private readonly WinBizCommerceApiService _service;

        public WinBizCommerceApiServiceTests()
        {
            var settingsService = new SettingsService(new WinBizApiSettings(GetEnvironmentVariable("WINBIZ_API_KEY"),
                GetEnvironmentVariable("WINBIZ_API_COMPANY"),
                GetEnvironmentVariable("WINBIZ_API_USERNAME"),
                GetEnvironmentVariable("WINBIZ_API_PASSWORD"),
                "BgIAAACkAABSU0ExAAQAAAEAAQBZ3myd6ZQA0tUXZ3gIzu1sQ7larRfM5KFiYbkgWk+jw2VEWpxpNNfDw8M3MIIbbDeUG02y/ZW+XFqyMA/87kiGt9eqd9Q2q3rRgl3nWoVfDnRAPR4oENfdXiq5oLW3VmSKtcBl2KzBCi/J6bbaKmtoLlnvYMfDWzkE3O1mZrouzA==",
                "https://api.winbizcloud.ch/"));
            _service = new WinBizCommerceApiService(settingsService.WinBizApiSettings, "BizyBoard", 2, 2018, new Logger<WinBizCommerceApiService>(new LoggerFactory()));
        }

        [Fact]
        public async Task GetStock_ReturnsStock_WhenProductExists()
        {
            var response = await _service.Stock(108).ConfigureAwait(false);

            Assert.True(response.Value == 100);
        }

        [Fact]
        public async Task Adresses_ReturnsAddressesList_WhenNoLimitDateIsSet()
        {
            var response = await _service.Addresses().ConfigureAwait(false);

            Assert.True(response.Values.Any());
        }

        [Fact]
        public async Task AdInfo_CustomerBalanceMethod_ReturnsValue()
        {
            var response = await _service.AdInfo(AdInfoMethodsEnum.CustomerBalance, 18).ConfigureAwait(false);

            Assert.True(response.Value == 240565);
        }

        [Fact]
        public async Task AdInfo_CustomerInvoicesOpen_ReturnsValue()
        {
            var response = await _service.AdInfo(AdInfoMethodsEnum.CustomerInvoicesOpen, 18).ConfigureAwait(false);

            Assert.True(response.Value == 4);
        }

        [Fact]
        public async Task AdInfo_SalesCount_ReturnsValue()
        {
            var response = await _service.AdInfo(AdInfoMethodsEnum.SalesCount, 18).ConfigureAwait(false);

            Assert.True(response.Value == 4);
        }

        [Fact]
        public async Task AdInfo_CustomerSalesItem_ReturnsValue()
        {
            var response = await _service.AdInfo(AdInfoMethodsEnum.CustomerSalesItem, 18, vStock: "SERVICES").ConfigureAwait(false);

            Assert.True(response.Value == 021509M);
        }
    }
}