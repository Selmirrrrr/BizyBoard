namespace Bizy.Web.Services
{
    using Models;

    public class SettingsService
    {
        public WinBizApiSettings WinBizApiSettings { get; }

        public SettingsService(WinBizApiSettings winBizApiSettings)
        {
            WinBizApiSettings = winBizApiSettings;
        }
    }
}
