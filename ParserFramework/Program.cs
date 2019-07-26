using ParserFramework.Core;
using ParserFramework.ParseRules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework
{
    class Program
    {
        static ParseRule Attribuition => new GroupRule
        {
            rules = new List<ParseRule>()
            {
                new TokenRule<IdToken>("var_name"),
                new SymbolRule("="){ignore=true},
                new TokenRule<NumberToken>("value")
            }
        };

        static void Main(string[] args)
        {
            var input = "-1 ++ 13--8*2--3.14";
            
            var list = Expression.Parser.DefaultTokenList(input);
            var rule = Expression.Parser.AdditionRule;
            rule.kind = ParseRule.Kind.Multiple;

            var info = rule.Execute(list);
            if (info == null) Console.WriteLine("NOOPE");
            Console.WriteLine(info);
            try
            {
                Expression.Solver.TrySolve(input, out float result);
                Console.WriteLine("result = " + result);
            }
            catch (ArgumentException) { }
            Console.WriteLine();

            Console.ReadKey(true);
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

        public static ParseRule StringRule => new AlternateRule
        {
            name = "string",
            possibilities = new List<ParseRule>()
            {
                new GroupRule()
                {
                    rules = new List<ParseRule>()
                    {
                        new SymbolRule("\"") { ignore = true },
                        new TokenRule<IdToken>("value"),
                        new SymbolRule("\"") { ignore = true },
                    }
                },
                new GroupRule()
                {
                    rules = new List<ParseRule>()
                    {
                        new SymbolRule("'") { ignore = true },
                        new TokenRule<IdToken>("value"),
                        new SymbolRule("'") { ignore = true },
                    }
                }
            }
        };

        public static ParsingInfo String(TokenList list)
        {
            //string :: Symbol(") Identifier Symbol(")

            return StringRule.Execute(list);
        }

        static TokenList DefaultTokenList(string input)
        {
            Tokenizer tokenizer = new Tokenizer(new StringReader(input));
            tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
            tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
            tokenizer.rules.Add(new Regex(@"^(\w+)"), m => new IdToken(m.Value));

            TokenList list = new TokenList(tokenizer);
            return list;
        }

    }
}