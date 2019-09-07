using ParserFramework.Core;
using ParserFramework.Core.ParseRules;
using System;
using System.Collections.Generic;
using System.IO;

namespace ParserFramework.Examples.CPTest
{
    public static class Main
    {
        public static void Run()
        {
            Do("if (2)");
            Do("if (a)");
            Do("while (a)");
            Do("while (2)");
            Do("exit");

            Console.ReadKey(true);
        }

        static void Do(string input)
        {
            var tokens = Rules.TokensFor(input);
            Console.WriteLine(tokens);
            var rule = Rules.Main;
            var info = rule.Execute(tokens);
            if (info)
            {
                Console.WriteLine(info.ToString());
            }
            else
            {
                Console.WriteLine(rule.LastErrors.ReduceToString("\n"));
                if(Rules.Main.checkPoint!=null)
                {
                    Console.WriteLine(rule.checkPoint.Descriptor);
                }
            }
            Console.WriteLine();
        }
    }

    public static class Rules
    {
        public static ParseRule Main => Cmds;

        static ParseRule Cmds => new AlternateRule("Cmds")
        {
            possibilities = new List<ParseRule>()
            {
                IfCmd,
                WhileCmd,
                new ReservedWordRule("exit"),
            }
        };

        static ParseRule IfCmd => new GroupRule("If")
        {
            rules = new List<ParseRule>()
            {
                new ReservedWordRule("if"), // there's no other rule that starts with 'if', so if the input passes this rule
                // we should only show errors for this rule
                
                new CheckPointRule("if cmd"), // prevents higher AlternateRules from trying next possibilities should this one fail

                new SymbolRule("("),
                new NumberRule(),
                new SymbolRule(")"),
            }
        };


        static ParseRule WhileCmd => new GroupRule("While")
        {
            rules = new List<ParseRule>()
            {
                new ReservedWordRule("while"),
                new CheckPointRule("while cmd"),

                new SymbolRule("("),
                new IdRule(),
                new SymbolRule(")"),
            }
        };

        public static TokenList TokensFor(string input)
        {
            Tokenizer tokenizer = new Tokenizer(new StringReader(input));

            tokenizer.AddDefaultRuleForInteger();
            tokenizer.AddDefaultRuleForIdentifier();
            
            tokenizer.AddSpecialRule(c =>
            {
                if (char.IsSymbol(c) || char.IsPunctuation(c))
                {
                    return new SymbolToken(c);
                }
                return null;
            });
            tokenizer.ignore = c => char.IsWhiteSpace(c);

            return new TokenList(tokenizer);
        }
    }
}
