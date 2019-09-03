using ParserFramework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework
{
    class MainClass
    {
        public static void Main()
        {
            while(true)
            {
                string input = Console.ReadLine();

                var command = Examples.Script.Parser.Parse(input);
                Console.WriteLine("Command: " + command);
            }
        }
    }
    class Program
    {
        public class StringToken : Token
        {
            public readonly string Value;
            public StringToken(string value) : base(Kind.CUSTOM)
            {
                this.Value = value;
            }
            public override string ToString()
            {
                return "STRING " + Value;
            }
        }

        public static TokenList DefaultTokenList(string input)
        {
            Tokenizer tokenizer = new Tokenizer(new StringReader(input));
            tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
            tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
            tokenizer.rules.Add(new Regex(@"^(\w+)"), m => new IdToken(m.Value));
            
            tokenizer.rules.Add(new Regex(@"\'.*\'"), m => new StringToken(m.Value));
            tokenizer.rules.Add(new Regex("\".*\""), m => new StringToken(m.Value));


            TokenList list = new TokenList(tokenizer);
            return list;
        }

    }

    public static class Utils
    {
        public static string ReduceToString<T>(this IEnumerable<T> en, Func<T, string> ToString, string separator)
        {
            string rt = "";
            foreach (var t in en)
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
