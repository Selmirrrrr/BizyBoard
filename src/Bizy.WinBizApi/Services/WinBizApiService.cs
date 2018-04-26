namespace Bizy.WinBizApi.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Extensions;
    using Models;
    using Newtonsoft.Json;
    using RestEase;

    public class WinBizApiService
    {
        private readonly IWinBizApi _api;

        public WinBizApiService(WinBizApiSettings winBizApiSettings, int companyId, int year)
        {
            _api = RestClient.For<IWinBizApi>(winBizApiSettings.Url);
            _api.CompanyName = winBizApiSettings.Company;
            _api.Username = winBizApiSettings.Username;
            _api.Password = winBizApiSettings.Password.Encrypt(winBizApiSettings.EncryptionKey);
            _api.CompanyId = companyId;
            _api.Year = year;
            _api.Key = winBizApiSettings.Key;
        }

        public async Task<string> GetStock(int arCode)
        {
            try
            {
                var req = new GetStockParams("disponible", arCode, new DateTime(2017, 2, 20));

                var lol = JsonConvert.SerializeObject(req);

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