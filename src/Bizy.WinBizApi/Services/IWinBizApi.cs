namespace Bizy.WinBizApi.Services
{
    using System.Threading.Tasks;
    using Bizy.WinBizApi.Models;
    using Refit;

    public interface IWinBizCommerceApi
    {
        [Post("/Bizinfo")]
        Task<T> Test<T>([Body] BaseRequest req,
            [Header("winbiz-companyname")] string winbizCompanyName,
            [Header("winbiz-username")] string winbizUsername,
            [Header("winbiz-password")] string winbizPassword,
            [Header("winbiz-companyid")] int winbizCompanyId,
            [Header("winbiz-year")] int winbizYear,
            [Header("winbiz-key")] string winbizKey
        );
    }
}