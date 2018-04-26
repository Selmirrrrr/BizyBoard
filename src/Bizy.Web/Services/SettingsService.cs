namespace Bizy.Web.Services
{
    using WinBizApi;

    public class SettingsService : ISettingsService
    {
        public WinBizApiSettings WinBizApiSettings { get; }

        public SettingsService(WinBizApiSettings winBizApiSettings)
        {
            WinBizApiSettings = winBizApiSettings;
        }
    }
}
