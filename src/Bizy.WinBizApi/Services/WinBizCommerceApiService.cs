namespace Bizy.WinBizApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Threading.Tasks;
    using Extensions;
    using Bizy.WinBizApi.Models;
    using Newtonsoft.Json;
    using Refit;

    public class WinBizCommerceApiService
    {
        private readonly WinBizApiSettings _winBizApiSettings;
        private readonly int _companyId;
        private readonly int _year;
        private readonly IWinBizCommerceApi _api;

        public WinBizCommerceApiService(WinBizApiSettings winBizApiSettings, int companyId, int year)
        {
            _winBizApiSettings = winBizApiSettings;
            _companyId = companyId;
            _year = year;
            _api = RestService.For<IWinBizCommerceApi>(winBizApiSettings.Url);
        }

        [MethodName("Stock")]
        public async Task<ValueResponse> Stock(int nItem, DateTime? dDateEnd = null, DateTime? dDateStart = null, int? nWarehouse = null, DateTime? dExpiryEnd = null, DateTime? dExpiryStart = null)
        {
            var parameters = new object[]
                    {"disponible", nItem, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), nWarehouse?.ToString(), dExpiryEnd?.ToWinBizString(), dExpiryStart?.ToWinBizString()}
                .AsEnumerable()
                .Where(p => p != null).ToArray();

            return await RequestAsync<ValueResponse>(new BaseRequest(parameters));
        }

        public async Task<ListResponse<Address>> Addresses(DateTime? dDateSince = null)
        {
            var parameters = new object[] { dDateSince?.ToWinBizString() }.AsEnumerable().Where(p => p != null).ToArray();

            return await RequestAsync<ListResponse<Address>>(new BaseRequest(parameters));
        }

        public async Task<T> RequestAsync<T>(BaseRequest request)
        {
            try
            {
                return await _api.Test<T>(request, _winBizApiSettings.Company, _winBizApiSettings.Username, _winBizApiSettings.Password.Encrypt(_winBizApiSettings.EncryptionKey), _companyId, _year, _winBizApiSettings.Key).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                //TODO Log
                return default(T);
            }
        }
    }
}