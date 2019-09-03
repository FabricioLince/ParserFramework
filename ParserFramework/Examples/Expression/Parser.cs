using ParserFramework.Core;
using ParserFramework.Core.ParseRules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework.Examples.Expression
{
    public class Parser
    {
        public static ParseRule Main => AdditionRule;

        public static ParseRule AdditionRule => new GroupRule()
        {
            name = "Add",
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
                new GroupRule()
                {
                    name = "sub_expr",
                    rulesF = new List<Func<ParseRule>>()
                    {
                        () => new SymbolRule("+", "-") { name = "signal", kind = ParseRule.Kind.Optional },
                        () => new SymbolRule("("){ ignore=true },
                        () => AdditionRule,
                        () => new SymbolRule(")"){ ignore=true },
                    }
                }
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

        public static TokenList DefaultTokenList(string input)
        {
            Tokenizer tokenizer = new Tokenizer(new StringReader(input));
            tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
            tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
            tokenizer.rules.Add(new Regex(@"^([a-z]+)"), m => new IdToken(m.Value));

            return new TokenList(tokenizer);
        }
    }
}
