namespace Bizy.WinBizApi.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Bizy.WinBizApi.Models;
    using Microsoft.Extensions.Logging;
    using Refit;

    public class WinBizCommerceApiService
    {
        private readonly WinBizApiSettings _winBizApiSettings;
        private readonly int _companyId;
        private readonly int _year;
        private readonly ILogger<WinBizCommerceApiService> _logger;
        private readonly IWinBizApi _api;

        public WinBizCommerceApiService(WinBizApiSettings winBizApiSettings, int companyId, int year, ILogger<WinBizCommerceApiService> logger)
        {
            _winBizApiSettings = winBizApiSettings;
            _companyId = companyId;
            _year = year;
            _logger = logger;
            _api = RestService.For<IWinBizApi>(winBizApiSettings.Url);
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
                _logger.LogError(e, "La requête a échoué", request);
                return default(T);
            }
        }
    }
}