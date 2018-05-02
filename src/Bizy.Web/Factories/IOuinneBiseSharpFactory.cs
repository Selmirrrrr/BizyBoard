namespace Bizy.Web.Factories
{

    using Bizy.OuinneBiseSharp.Services;

    public interface IOuinneBiseSharpFactory
    {
        OuinneBiseSharpService GetService(string winBizCompanyName, string winBizUsername, string winBizPassword, int winBizCompanyId, int winBizYear, string winBizKey, string appName);
    }
}