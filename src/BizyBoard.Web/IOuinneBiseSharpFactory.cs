using Bizy.OuinneBiseSharp.Services;

namespace BizyBoard.Web
{
    public interface IOuinneBiseSharpFactory
    {
        OuinneBiseSharpService GetInstance(string winBizUsername, string winBizpassword);
    }
}