namespace Bizy.Tests
{
    using System.Threading.Tasks;
    using Services;
    using Xunit;

    public class WinBizApiServiceUnitTests
    {
        private readonly WinBizApiService _service;

        public WinBizApiServiceUnitTests()
        {
            _service = new WinBizApiService("Company", "UserName", "Pwd", 1, 2017);
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
    }
}
