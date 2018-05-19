using System.Threading.Tasks;
using Bizy.OuinneBiseSharp.Services;
using BizyBoard.Models.DbEntities;

namespace BizyBoard.Web
{
    public interface IOuinneBiseSharpFactory
    {
        OuinneBiseSharpService GetInstance(string winBizCompany, string winBizUsername, string winBizpassword);
        OuinneBiseSharpService GetInstance(AppUser user);
    }
}