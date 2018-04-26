namespace Bizy.WinBizApi.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static string ToWinBizString(this DateTime date) => date.ToString("yyyy-MM-dd");
    }
}
