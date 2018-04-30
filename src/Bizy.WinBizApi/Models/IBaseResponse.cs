using Bizy.WinBizApi.Enums;

namespace Bizy.WinBizApi.Models
{
    public interface IBaseResponse
    {
        int? ErrorLast { get; set; }
        ErrorLevelEnum ErrorLevel { get; set; }
        int? ErrorsCount { get; set; }
        string ErrorsMsg { get; set; }
        string UserErrorMsg { get; set; }
    }
}