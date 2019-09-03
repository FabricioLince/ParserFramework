using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ParserFramework.Core.ParseRules;
using ParserFramework.Core;

namespace ParserFramework.Examples.Equation
{
    public class Rules
    {
        public static ParseRule Equation = new GroupRule
        {
            name = "Equation",
            rules = new List<ParseRule>()
            {
                AdditionRule,
                new SymbolRule("=") { name = "equality" },
                AdditionRule,
            }
        };

        static ParseRule AdditionRule => new GroupRule
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

        static ParseRule MultiplicationRule => new GroupRule
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

        static ParseRule Term => new AlternateRule
        {
            name = "Term",
            possibilities = new List<ParseRule>()
            {
                XTerm,
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

        static ParseRule XTerm => new AlternateRule
        {
            name = "XTerm",
            possibilities = new List<ParseRule>()
            {
                new GroupRule()
                {
                    name = "XTerm",
                    rules = new List<ParseRule>()
                    {
                        Number,
                        new IdRule("x"){ name = "var" }
                    }
                },
                new GroupRule()
                {
                    name = "XTerm",
                    rules = new List<ParseRule>()
                    {
                        new SymbolRule("+", "-"){ name = "signal", kind = ParseRule.Kind.Optional},
                        new IdRule("x"){ name = "var" }
                    }
                }
            }
        };

        static ParseRule Number => new GroupRule
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

        public class Exception : System.Exception
        {
            public List<ParseErrorInfo> AllErrors { get; private set; } = new List<ParseErrorInfo>();
            public List<ParseErrorInfo> LastErrors { get; private set; } = new List<ParseErrorInfo>();
            public ParseRule Rule { get; private set; }

            public Exception(ParseRule rule) : base(rule.Descriptor + " could not be executed on input")
            {
                AllErrors.AddRange(rule.errorInfo);
                LastErrors.AddRange(rule.LastErrors);
                Rule = rule;
            }
        }
    }
}

