namespace Bizy.Web.Services
{
    using WinBizApi;

    public interface ISettingsService
    {
        WinBizApiSettings WinBizApiSettings { get; }
    }
}