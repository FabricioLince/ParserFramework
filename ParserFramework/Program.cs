using ParserFramework.Core;
using ParserFramework.Equation;
using ParserFramework.ParseRules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class MainClass
{
    public static void Main(string[] args)
    {
        string input = "1--(-2)=(-2x--x)*2";
        Console.WriteLine(input);
        if (Solver.TrySolve(input, out float result))
        {
            Console.WriteLine("x = " + result);
        }

        Console.ReadKey(true);
    }
}

namespace ParserFramework
{
    using N = Equation;

    class Program
    {
        static void Main2(string[] args)
        {
            var input = "2*x=12";

            input = Equation.Expander.Expand(input);
            Console.WriteLine(input);

            TokenList list = Expression.Parser.DefaultTokenList(input);
            var rule = N.Rules.Main;

            var info = rule.Execute(list);
            if (info == null) Console.WriteLine("not an " + rule.name);

            if (list.Current.kind != Token.Kind.EOF || rule.Error)
            {
                Console.WriteLine("Stopped on " + list.Current + " " +list.Current.Position);

                var error = "";
                for (int i = 0; i < rule.errorInfo.Count; i++)
                {
                    error += rule.errorInfo[i].ToString();
                }
                Console.WriteLine("ERROR:\n" + error);
            }

            Console.WriteLine(info);
            try
            {
                N.Solver.TrySolve(input, out float result);
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

    public static class Utils
    {
        public static string ReduceToString<T>(this IEnumerable<T> en, Func<T, string> ToString, string separator)
        {
            string rt = "";
            foreach(var t in en)
            {
                rt += ToString(t) + separator;
            }
            return rt;
        }
        public static string ReduceToString<T>(this IEnumerable<T> en, string separator) where T : class
        {
            string rt = "";
            foreach (var t in en)
            {
                rt += t.ToString() + separator;
            }
            return rt;
        }

        public static string Repeat(this string str, int times)
        {
            string rt = "";
            for (int i = 0; i < times; i++)
            {
                rt += str;
            }
            return rt;
        }
    }
}

/*
 <number>
gets a token of type NumberToken
<symbol>
gets a token of type SymbolToken
<symbol + - >
gets a token of type SymbolToken only if the symbol
caught matchs one of the symbols specified
:+
syntatic sugar for <symbol + >
<id>
gets a token of type IdToken

RuleName = Rules
creates a rule named RuleName comprised of the rules
specified in Rules

Add = Mult expr_op*
expr_op = <symbol + - > Mult
Mult = Term fator_op*
fator_op = <symbol * / > Term
Term = (Number | sub_expr)
sub_expr = [<symbol + - >] :( Add :)
Number = [<symbol + - >] <number>
     
     */
