namespace Bizy.WinBizApi.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static string ToWinBizString(this DateTime date) => date.ToString("dd.MM.yyyy");
    }
}
