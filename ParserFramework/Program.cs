using ParserFramework.Core;
using ParserFramework.Expression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] testCases = new string[]
            {
                "-12+1*2*3",
                "1-1-2+1",
                "1*22",
                "\"test\"",
                "11",
                "2=2",
                "4/2",
                "abs",
                "-15",
                "+1337",
                "++1",
                "1.1+1+-3.14-+1"
            };
            
            foreach (string testCase in testCases)
            {
                TokenList list = DefaultTokenList(testCase);
                list.MoveNext();

                Console.WriteLine("Testing: " + testCase);
                ParsingInfo info = String(list);
                if (info != null)
                {
                    Console.WriteLine("Is string");
                }
                else
                {
                    if (Solver.TrySolve(testCase, out float result))
                    {
                        Console.WriteLine("result = " + result);
                    }
                    else
                    {
                        Console.WriteLine("dont know");
                    }
                }
                Console.WriteLine();
            }

            string input = "number :: [SymbolToken('+')] NumberToken";
            var tokens = RuleCreator.GetTokens(input);
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }

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

        public static ParsingInfo String(TokenList list)
        {
            //string :: Symbol(") Identifier Symbol(")

            GroupRule rule = new GroupRule
            {
                name = "string",
                kind = ParseRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    new TokenRule(TokenParser.Symbol, "\""),
                    new TokenRule(TokenParser.Identifier),
                    new TokenRule(TokenParser.Symbol, "\"")
                }
            };

            return rule.Execute(list);
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