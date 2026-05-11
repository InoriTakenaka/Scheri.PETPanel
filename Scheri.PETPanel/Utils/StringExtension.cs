using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Scheri.PETPanel.Utils {
    public static class StringExtension {
        public static bool In(this string str, string[] values) {
            return values.Contains(str);
        }

        public static string HasValue(this string str, string defaultValue) {
            if (string.IsNullOrEmpty(str)) {
                return defaultValue;
            }
            return str;
        }
    }
}
