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
        public static void AddDefaultRuleForInteger(this Tokenizer tokenizer)
        {
            tokenizer.AddRegexRule(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
        }

        public static void AddDefaultRuleForFloat(this Tokenizer tokenizer)
        {
            tokenizer.AddRegexRule(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
        }

        public static void AddDefaultRuleForIdentifier(this Tokenizer tokenizer)
        {
            tokenizer.AddRegexRule(new Regex(@"^(\w+)"), m => new IdToken(m.Value));
        }

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


        public static void AddSymbolRule(this Tokenizer tokenizer, string symbolString)
        {
            tokenizer.rules.Add(Utils.RegexForSymbols(symbolString), m => new SymbolToken(symbolString));
        }
    }
}
