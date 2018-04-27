namespace Bizy.WinBizApi
{
    using System;

    class MethodName : Attribute
    {
        public string Method { get; set; }

        public MethodName(string method)
        {
            Method = method;
        }
    }
}