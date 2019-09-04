using System;
using System.Collections.Generic;
using ParserFramework.Core.ParseRules;

namespace ParserFramework.Examples.Script
{
    partial class Rules
    {
        public static ParseRule Expression(string name) => new GroupRule(name)
        {
            kind = ParseRule.Kind.Mandatory,
            rules = new List<ParseRule>()
            {
                MultiplicationRule,
                new GroupRule()
                {
                    name = "add_op",
                    kind = ParseRule.Kind.Multiple,
                    rules = new List<ParseRule>()
                    {
                        new SymbolRule("+", "-") { name = "op" },
                        MultiplicationRule,
                    }
                }
            }
        };

        public static ParseRule MultiplicationRule => new GroupRule
        {
            name = "Mult",
            rules = new List<ParseRule>()
            {
                Term,
                new GroupRule()
                {
                    name = "mult_op",
                    kind = ParseRule.Kind.Multiple,
                    rules = new List<ParseRule>()
                    {
                        new SymbolRule("*", "/") { name = "op" },
                        Term
                    }
                }
            }
        };

        public static ParseRule Term => new AlternateRule
        {
            name = "Term",
            possibilities = new List<ParseRule>()
            {
                Number,
                Variable,
                new GroupRule()
                {
                    name = "sub_expr",
                    rulesF = new List<Func<ParseRule>>()
                    {
                        () => new SymbolRule("+", "-") { name = "signal", kind = ParseRule.Kind.Optional },
                        () => new SymbolRule("("){ ignore=true },
                        () => Expression("Add"),
                        () => new SymbolRule(")"){ ignore=true },
                    }
                },
            }
        };

        public static ParseRule Number => new GroupRule
        {
            name = "Number",
            rules = new List<ParseRule>()
            {
                new SymbolRule("+", "-") { name = "signal", kind = ParseRule.Kind.Optional },
                new NumberRule() { name = "value" }
            }
        };

        static ParseRule Variable = new GroupRule("Variable")
        {
            rules = new List<ParseRule>()
            {
                new IdRule(){name ="varName"}
            }
        };
    }
}