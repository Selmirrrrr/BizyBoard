namespace BizyBoard.Web
{
    using Bizy.OuinneBiseSharp.Services;
    using Models.DbEntities;

    public interface IOuinneBiseSharpFactory
    {
        OuinneBiseSharpService GetInstance(string winBizCompany, string winBizUsername, string winBizpassword);
        OuinneBiseSharpService GetInstance(string company, AppUser user);
    }
}