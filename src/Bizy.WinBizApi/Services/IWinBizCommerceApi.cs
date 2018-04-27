namespace Bizy.WinBizApi.Services
{
    using System.Threading.Tasks;
    using Models;
    using RestEase;

    public interface IWinBizCommerceApi
    {
        [Header("winbiz-companyname")]
        string CompanyName { get; set; }

        [Header("winbiz-username")]
        string Username { get; set; }

        [Header("winbiz-password")]
        string Password { get; set; }

        [Header("winbiz-companyid")]
        int CompanyId { get; set; }

        [Header("winbiz-year")]
        int Year { get; set; }

        [Header("winbiz-key")]
        string Key { get; set; }

        [Post("Bizinfo")]
        Task<ValueResponse> Stock([Body] BaseRequest req);

        [Post("Bizinfo")]
        Task<ListResponse<Address>> Addresses([Body] BaseRequest req);

        [Post("Bizinfo")]
        Task<T> Test<T>([Body] BaseRequest req);
    }
}