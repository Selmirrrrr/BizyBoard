namespace BizyBoard.Web
{
    using System;
    using Bizy.OuinneBiseSharp.Services;

    public class OuinneBiseSharpFactory : IOuinneBiseSharpFactory
    {
        public OuinneBiseSharpService GetInstance(string winBizCompany, string winBizUsername, string winBizpassword) => new OuinneBiseSharpService(winBizCompany, Environment.GetEnvironmentVariable("WINBIZ_API_USERNAME"), Environment.GetEnvironmentVariable("WINBIZ_API_PASSWORD"), 2, 2018, Environment.GetEnvironmentVariable("WINBIZ_API_KEY"), "BizyBoard");
    }
}