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

    public class WinBizCommerceApiService
    {
        private readonly IWinBizCommerceApi _api;

        public WinBizCommerceApiService(WinBizApiSettings winBizApiSettings, int companyId, int year)
        {
            _api = RestClient.For<IWinBizCommerceApi>(winBizApiSettings.Url);
            _api.CompanyName = winBizApiSettings.Company;
            _api.Username = winBizApiSettings.Username;
            _api.Password = winBizApiSettings.Password.Encrypt(winBizApiSettings.EncryptionKey);
            _api.CompanyId = companyId;
            _api.Year = year;
            _api.Key = winBizApiSettings.Key;
        }

        [MethodName("Stock")]
        public async Task<string> Stock(int nItem, DateTime? dDateEnd = null, DateTime? dDateStart = null, int? nWarehouse = null, DateTime? dExpiryEnd = null, DateTime? dExpiryStart = null)
        {
            try
            {
                var parameters = new object[]
                        {"disponible", nItem, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), nWarehouse?.ToString(), dExpiryEnd?.ToWinBizString(), dExpiryStart?.ToWinBizString()}
                    .AsEnumerable()
                    .Where(p => p != null).ToArray();

                var req = new BaseRequest(parameters);

                var result = await _api.Stock(req).ConfigureAwait(false);

                return result.Value;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public async Task<IList<Address>> Addresses(DateTime? dDateSince = null)
        {
            try
            {
                var parameters = new object[] { dDateSince?.ToWinBizString() }.AsEnumerable().Where(p => p != null).ToArray();

                var req = new BaseRequest(parameters);

                var r = JsonConvert.SerializeObject(req);

                var result = await _api.Test<ListResponse<Address>>(req).ConfigureAwait(false);
                //var result = await _api.Addresses(req).ConfigureAwait(false);

                return new List<Address>();
            }
            catch (Exception e)
            {
                //TODO Log
                return new List<Address>();
            }
        }

        public string GetMethodName<T>()
        {
            if (typeof(T).GetCustomAttributes(typeof(MethodName), true).FirstOrDefault() is MethodName dnAttribute) return dnAttribute.Method;
            return null;
        }
    }
}