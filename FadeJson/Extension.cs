using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FadeJson
{
    public static class Extension
    {
        public static string TrimHeadAndTail(this string str) {
            return str?.Substring(1, str.Length - 2);
        }

    }
}
