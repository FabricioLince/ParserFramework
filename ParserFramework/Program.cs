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
                "12+1",
                "1-1",
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
                    var expr = Parser.AdditionRule().Execute(list);
                    if (expr != null)
                    {
                        if ((expr.tokens["op"] as SymbolToken).Value == "+")
                            Console.WriteLine("is addition");
                        else
                            Console.WriteLine("is subtraction");
                        Console.WriteLine(expr);
                    }
                    else
                    {
                        expr = Parser.Multiplication(list);
                        if (expr != null)
                        {
                            if ((expr.tokens["operator"] as SymbolToken).Value == "*")
                                Console.WriteLine("is multiplication");
                            else
                                Console.WriteLine("is division");
                        }
                        else
                        {
                            info = Parser.NumberRule().Execute(list);
                            if (info != null)
                            {
                                Console.WriteLine("is number");
                                Console.WriteLine(info);
                            }
                        }
                    }
                }
                //Console.WriteLine(expr);
                Console.WriteLine();
            }

            Console.ReadKey(true);
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