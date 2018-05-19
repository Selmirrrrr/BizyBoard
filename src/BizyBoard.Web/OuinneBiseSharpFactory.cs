namespace BizyBoard.Web
{
    using System;
    using System.Threading.Tasks;
    using Bizy.OuinneBiseSharp.Services;
    using Models.DbEntities;

    public class OuinneBiseSharpFactory : IOuinneBiseSharpFactory
    {
        public OuinneBiseSharpService GetInstance(string winBizCompany, string winBizUsername, string winBizpassword) => new OuinneBiseSharpService(winBizCompany, winBizUsername, winBizpassword, 2, 2018, Environment.GetEnvironmentVariable("WINBIZ_API_KEY"), "BizyBoard");
        public OuinneBiseSharpService GetInstance(AppUser user) => new OuinneBiseSharpService(user.Tenant.Name, user.ErpUsername, user.ErpPassword, user.LastErpCompanyId, user.LastErpFiscalYear, Environment.GetEnvironmentVariable("WINBIZ_API_KEY"), "BizyBoard");
    }
}