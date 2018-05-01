namespace Bizy.WinBizApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Bizy.WinBizApi.Models;
    using Enums;
    using Microsoft.Extensions.Logging;
    using Refit;

    public class WinBizCommerceApiService
    {
        public readonly WinBizApiSettings WinBizApiSettings;
        private readonly string _appName;
        private readonly int _companyId;
        private readonly int _year;
        private readonly ILogger<WinBizCommerceApiService> _logger;
        private readonly IWinBizApi _api;

        public WinBizCommerceApiService(WinBizApiSettings winBizApiSettings, string appName, int companyId, int year, ILogger<WinBizCommerceApiService> logger)
        {
            WinBizApiSettings = winBizApiSettings;
            _appName = appName;
            _companyId = companyId;
            _year = year;
            _logger = logger;
            _api = RestService.For<IWinBizApi>(winBizApiSettings.Url);
        }

        public async Task<Response<int>> Stock(int nItem, DateTime? dDateEnd = null, DateTime? dDateStart = null, int? nWarehouse = null, DateTime? dExpiryEnd = null, DateTime? dExpiryStart = null)
        {
            var parameters = new object[]
                    {"disponible", nItem, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), nWarehouse?.ToString(), dExpiryEnd?.ToWinBizString(), dExpiryStart?.ToWinBizString()}
                .AsEnumerable()
                .Where(p => p != null).ToArray();

            return await RequestAsync<Response<int>>(new BaseRequest(parameters));
        }

        public async Task<Response<List<Address>>> Addresses(DateTime? dDateSince = null)
        {
            var parameters = new object[] { dDateSince?.ToWinBizString() }.AsEnumerable().Where(p => p != null).ToArray();

            return await RequestAsync<Response<List<Address>>>(new BaseRequest(parameters));
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
        public async Task<Response<decimal>> AdInfo(AdInfoMethodsEnum method, int nAdresse, DateTime? dDateEnd = null, DateTime? dDateStart = null, string vStock = null)
        {
            var parameters = new object[] { method.ToDescriptionString(), nAdresse, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), vStock }.AsEnumerable().Where(p => p != null).ToArray();

            return await RequestAsync<Response<decimal>>(new BaseRequest(parameters));

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
        public async Task<Response<int>> AdInfo(AdInfoMethodsEnum method, int nAdresse, int vStock, DateTime? dDateEnd = null, DateTime? dDateStart = null)
        {
            var parameters = new object[] { method.ToDescriptionString(), nAdresse, dDateEnd?.ToWinBizString(), dDateStart?.ToWinBizString(), vStock }.AsEnumerable().Where(p => p != null).ToArray();

            return await RequestAsync<Response<int>>(new BaseRequest(parameters));

        }

        public async Task<Response<List<Dossier>>> Folders() => await RequestAsync<Response<List<Dossier>>>(new BaseRequest());

        public async Task<T> RequestAsync<T>(BaseRequest request) where T : IBaseResponse
        {
            try
            {
                var result = await _api.Test<T>(request,
                                                WinBizApiSettings.Company,
                                                WinBizApiSettings.Username,
                                                WinBizApiSettings.Password.Encrypt(WinBizApiSettings.EncryptionKey),
                                                _companyId, _year, WinBizApiSettings.Key).ConfigureAwait(false);

                switch (result.ErrorLast ?? 0)
                {
                    case 109:
                    case 202:
                    case 239:
                    case 297:
                    case 490:
                    case 514:
                    case 734:
                    case 933:
                    case 963:
                    case 981:
                        result.ErrorLevel = ErrorLevelEnum.Developer;
                        result.UserErrorMsg = $"An error occurred in the application {_appName}";
                        break;
                    case 193:
                        result.ErrorLevel = ErrorLevelEnum.Customer;
                        result.UserErrorMsg = "Some of the following login details are missing: Name of the Company, ID of the Folder, Fiscal Year";
                        break;
                    case 314:
                        result.ErrorLevel = ErrorLevelEnum.Customer;
                        result.UserErrorMsg = "One of the login details is wrong or missing";
                        break;
                    case 803:
                        result.ErrorLevel = ErrorLevelEnum.Customer;
                        result.UserErrorMsg = "One of the login details is wrong or missing";
                        break;
                    case 335:
                        result.ErrorLevel = ErrorLevelEnum.Customer;
                        result.UserErrorMsg = $"This user is not authorized to use the application {_appName}";
                        break;
                    case 670:
                        result.ErrorLevel = ErrorLevelEnum.Customer;
                        result.UserErrorMsg = "The company selected by the user can't be found.";
                        break;
                    case 111:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = $"An error occurred in the application {_appName}";
                        break;
                    //case 134:
                    case 197:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 250:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 280:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    //case 297:
                    case 299:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 327:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 420:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    //case 514:
                    case 535:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 666:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 667:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 668:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 672:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 673:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 689:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 717:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 737:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 837:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 864:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 905:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                    case 999:
                        result.ErrorLevel = ErrorLevelEnum.WinBiz;
                        result.UserErrorMsg = "An error occurred in WinBIZ Cloud";
                        break;
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "La requête a échoué", request);
                return default(T);
            }
        }
    }
}