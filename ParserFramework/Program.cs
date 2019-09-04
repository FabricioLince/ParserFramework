using System;
using System.Collections.Generic;

namespace ParserFramework
{
    class MainClass
    {
        public static void Main()
        {
            Examples.Script.Main.Run();
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
            if(rt.Length>0)
                return rt.Substring(0, rt.Length - separator.Length);
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
