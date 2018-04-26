namespace Bizy.WinBizApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

        public async Task<string> GetStock(int nItem, DateTime? dDateEnd = null, DateTime? dDateStart = null, int? nWarehouse = null, DateTime? dExpiryEnd = null, DateTime? dExpiryStart = null)
        {
            try
            {
                var parameters = new object[]
                        {"disponible", nItem, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), nWarehouse?.ToString(), dExpiryEnd?.ToWinBizString(), dExpiryStart?.ToWinBizString()}
                    .AsEnumerable()
                    .Where(p => p != null).ToArray();

                var req = new BaseRequest("stock", parameters);

                var result = await _api.GetStock(req).ConfigureAwait(false);

                return result.Value;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public async Task<IList<Address>> Adresses(DateTime? dDateSince = null)
        {
            try
            {
                var parameters = new object[] { dDateSince?.ToWinBizString() }.AsEnumerable().Where(p => p != null).ToArray();

                var req = new BaseRequest("Addresses", parameters);

                var result = await _api.Addresses(req).ConfigureAwait(false);

                return result.Values;
            }
            catch (Exception e)
            {
                //TODO Log
                return new List<Address>();
            }
        }
    }
}