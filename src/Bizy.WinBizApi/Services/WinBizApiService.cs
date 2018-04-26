namespace Bizy.WinBizApi.Services
{
    using System;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using RestEase;

    public class WinBizApiService
    {
        private readonly IWinBizApi _api;
        private const string Key = "key";
        private const string Url = "url";

        public WinBizApiService(string companyName, string userName, string password, int companyId, int year)
        {
            _api = RestClient.For<IWinBizApi>(Url);
            _api.CompanyName = companyName;
            _api.Username = userName;
            _api.Password = password.Encrypt(Key);
            _api.CompanyId = companyId;
            _api.Year = year;
            _api.Key = "key";
        }

        public async Task<string> GetStock(int arCode)
        {
            try
            {
                var req = new BaseRequest("Stock", new BaseRequestParams
                {
                    Param1 = "disponible",
                    Param2 = arCode,
                    Param3 = "2017-02-20"
                });
                var result = await _api.GetStock(req).ConfigureAwait(false);
                return result.Value;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}