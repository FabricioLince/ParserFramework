using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserFramework.Core
{
    public static class Utils
    {
        static string PatternForSymbols(params string[] symbols)
        {
            string pattern = "^(";
            for (int i = 0; i < symbols.Length; ++i)
            {
                var symbolString = symbols[i];
                if (symbolString.Length == 1)
                {
                    pattern += "\\" + symbolString;
                }
                else
                {
                    string symbolPattern = "(?:";
                    foreach (var chara in symbolString)
                    {
                        symbolPattern += "\\" + chara;

                    }
                    pattern += symbolPattern + ")";
                }
                if (i < symbols.Length - 1) pattern += "|";
            }
            pattern += ")";
            return pattern;
            //return new Regex(pattern);
        }

        public static Regex RegexForSymbols(params string[] symbols)
        {
            return new Regex(PatternForSymbols(symbols));
        }
    }
}
