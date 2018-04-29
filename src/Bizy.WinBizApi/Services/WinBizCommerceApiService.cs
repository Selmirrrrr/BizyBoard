namespace Bizy.WinBizApi.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Bizy.WinBizApi.Models;
    using Microsoft.Extensions.Logging;
    using Refit;

    public partial class WinBizCommerceApiService
    {
        /// <summary>
        /// Test
        /// </summary>
        public readonly WinBizApiSettings WinBizApiSettings;
        private readonly int _companyId;
        private readonly int _year;
        private readonly ILogger<WinBizCommerceApiService> _logger;
        private readonly IWinBizApi _api;

        public WinBizCommerceApiService(WinBizApiSettings winBizApiSettings, int companyId, int year, ILogger<WinBizCommerceApiService> logger)
        {
            WinBizApiSettings = winBizApiSettings;
            _companyId = companyId;
            _year = year;
            _logger = logger;
            _api = RestService.For<IWinBizApi>(winBizApiSettings.Url);
        }

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

        /// <summary>
        /// This method can return various information related to an address.
        /// </summary>
        /// <param name="method">Piece of information to be returned. The possible values for cInfo are in <see cref="AdInfoMethodsEnum"/></param>
        /// <param name="nAdresse">Only the transactions concerning the addresses with ad_numero = nAdresse are considered.</param>
        /// <param name="dDateEnd">The transactions are selected up to the date specified. The parameter is optional.
        /// If the parameter is missing all the transactions are selected.</param>
        /// <param name="dDateStart">The transactions are selected starting from the date specified. The parameter is optional.
        /// If the parameter is missing all the transactions are selected</param>
        /// <param name="vStock">This parameter is used only if cInfo is customersalesitem or supplierpurchasesitem.
        /// If the type of vStock is a string, the cInfo is applied to the Items being in the group specified in vStock.</param>
        /// <returns></returns>
        public async Task<ValueResponse> AdInfo(AdInfoMethodsEnum method, int nAdresse, DateTime? dDateEnd = null, DateTime? dDateStart = null, string vStock = null)
        {
            var parameters = new object[] { method.ToDescriptionString(), nAdresse, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), vStock }.AsEnumerable().Where(p => p != null).ToArray();

            return await RequestAsync<ValueResponse>(new BaseRequest(parameters));

        }

        /// <summary>
        /// <see cref="AdInfo(AdInfoMethodsEnum,int,DateTime?,DateTime?,string)"/>
        /// </summary>
        /// <param name="method">Piece of information to be returned. The possible values for cInfo are in <see cref="AdInfoMethodsEnum"/></param>
        /// <param name="nAdresse">Only the transactions concerning the addresses with ad_numero = nAdresse are considered.</param>
        /// <param name="vStock">This parameter is used only if cInfo is customersalesitem or supplierpurchasesitem.
        /// If the type of vStock is a numeric, the cInfo is applied to the Item with ar_numero = vStock.</param>
        /// <param name="dDateEnd">The transactions are selected up to the date specified. The parameter is optional.
        /// If the parameter is missing all the transactions are selected.</param>
        /// <param name="dDateStart">The transactions are selected starting from the date specified. The parameter is optional.
        /// If the parameter is missing all the transactions are selected</param>
        /// <returns></returns>
        public async Task<ValueResponse> AdInfo(AdInfoMethodsEnum method, int nAdresse, int vStock, DateTime? dDateEnd = null, DateTime? dDateStart = null)
        {
            var parameters = new object[] { method.ToDescriptionString(), nAdresse, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), vStock }.AsEnumerable().Where(p => p != null).ToArray();

            return await RequestAsync<ValueResponse>(new BaseRequest(parameters));

        }

        public async Task<T> RequestAsync<T>(BaseRequest request)
        {
            try
            {
                return await _api.Test<T>(request, WinBizApiSettings.Company, WinBizApiSettings.Username, WinBizApiSettings.Password.Encrypt(WinBizApiSettings.EncryptionKey), _companyId, _year, WinBizApiSettings.Key).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "La requête a échoué", request);
                return default(T);
            }
        }
    }
}