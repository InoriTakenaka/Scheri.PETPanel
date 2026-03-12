using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace Scheri.PETPanel.Utils
{
    public static class StringExtension
    {
        public static byte[] ToAsciiBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static string ToAsciiString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static string ToJsonWithFormat(this object obj,JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(obj, options);
        }
    }
}
