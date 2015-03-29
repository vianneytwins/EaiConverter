using System;

namespace EaiConverter.Test.Utils
{
    public static class StringWindowsSupportExtension
    {
        public static string RemoveWindowsReturnLineChar(this String str)
        {
            return str.Replace("\r",string.Empty);
        }
    } 
}

