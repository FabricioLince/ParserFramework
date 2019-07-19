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
                "-12+1*2*3",/*
                "1-1-2+1",
                "1*22",
                "\"test\"",
                "11",/*
                "2=2",
                "4/2",
                "abs",
                "-15",
                "+1337",
                "++1",
                "1.1+1+-3.14-+1"*/
            };

            foreach (string testCase in testCases)
            {
                Tokenizer tokenizer = new Tokenizer(new StringReader(testCase));
                tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
                tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
                //tokenizer.rules.Add(new Regex(@"^(\+|\-|\*|\/|\=)"), m => new SymbolToken(m.Value[0]));
                tokenizer.rules.Add(new Regex(@"^([a-z]+)"), m => new IdToken(m.Value));

                TokenList list = new TokenList(tokenizer);
                list.MoveNext();

                Console.WriteLine("Testing: " + testCase);
                var info = Parser.String(list);
                if (info != null)
                {
                    Console.WriteLine("Is string");
                }
                else
                {
                    ParsingInfo expr = Parser.AdditionRule().Execute(list);
                    if (expr != null)
                    {
                        Console.WriteLine(expr);
                        SolveExpression(expr);
                    }
                    else
                    {
                        Console.WriteLine("dont know");
                    }
                }
                //Console.WriteLine(expr);
                Console.WriteLine();
            }

            Console.ReadKey(true);
        }

        static float SolveExpression(ParsingInfo expr)
        {
            if (expr.IsEmpty) return 0;
            int sum = 0;
            foreach (var pair in expr.info)
            {
                if(pair.Key == "fator")
                {
                    Console.WriteLine("Fator found ");
                    SolveFator(pair.Value as ParsingInfo.ChildInfo);
                }
            }

            return sum;
        }

        static float SolveFator(ParsingInfo.ChildInfo groupInfo)
        {
            foreach(var pair in groupInfo.child.info)
            {
                if(pair.Key == "number")
                {
                    Console.WriteLine("Number found " + pair.Value);
                    Console.WriteLine("= " + SolveNumber(pair.Value as ParsingInfo.ChildInfo));
                }
            }

            return 0;
        }

        static float SolveNumber(ParsingInfo.ChildInfo numberInfo)
        {
            float value = 0;

            var numberToken = numberInfo.child.info["value"].AsTokenInfo.token;
            if (numberToken is IntToken)
            {
                value = (numberToken as IntToken).Value;
            }
            else if (numberToken is FloatToken)
            {
                value = (numberToken as FloatToken).Value;
            }

            if(numberInfo.child.info.ContainsKey("signal"))
            {
                var signalToken = numberInfo.child.info["signal"].AsTokenInfo.token;
                var symbol = signalToken as SymbolToken;
                if(symbol!=null)
                {
                    if(symbol.Value == "-")
                    {
                        value *= -1;
                    }
                }
            }

            return value;
        }
    }

       
}
/*
 * 
            rule.name = "number";
            rule.groups.Add(new ParseRule.GroupRule()
            {
                kind = ParseRule.GroupRule.Kind.Optional,
                tokens = new List<ParseRule.TokenRule>()
                {
                    new ParseRule.TokenRule(Symbol, "+", "-")
                }
            });
            rule.groups.Add(new ParseRule.GroupRule()
            {
                kind = ParseRule.GroupRule.Kind.Mandatory,
                tokens = new List<ParseRule.TokenRule>()
                {
                    new ParseRule.TokenRule(Number)
                }
            });
*/