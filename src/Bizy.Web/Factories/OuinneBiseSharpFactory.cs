namespace Bizy.Web.Factories
{
    using System;
    using OuinneBiseSharp.Services;

    public class OuinneBiseSharpFactory : IOuinneBiseSharpFactory
    {
        public OuinneBiseSharpService GetService(string winBizCompanyName, string winBizUsername, string winBizPassword, int winBizCompanyId, int winBizYear, string winBizKey, string appName)
            => new OuinneBiseSharpService(winBizCompanyName, winBizUsername, winBizPassword, winBizCompanyId, winBizYear, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), appName);
    }
}
