using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromptRandomizer.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks if the given string is contained in a list of string
        /// </summary>
        /// <param name="list"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Contains(this List<string> list, string text)
        {
            foreach (var element in list)
                if (element.Equals(text))
                    return true;

            return false;
        }
    }
}
