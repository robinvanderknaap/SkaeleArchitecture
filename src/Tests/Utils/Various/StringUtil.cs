﻿namespace Tests.Utils.Various
{
    public static class StringUtil
    {
        public static string StripWhiteSpace(this string value)
        {
            return value
                .Replace(" ", "")
                .Replace("\t", "")
                .Replace("\r\n", "")
                .Replace("\n", "");
        }
    }
}
