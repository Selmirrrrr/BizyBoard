using Bizy.OuinneBiseSharp.Services;

namespace BizyBoard.Web
{
    public interface IOuinneBiseSharpFactory
    {
        OuinneBiseSharpService GetInstance(string winBizCompany, string winBizUsername, string winBizpassword);
    }
}