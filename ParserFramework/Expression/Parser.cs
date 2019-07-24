using ParserFramework.Core;
using ParserFramework.ParseRules;
using System.Collections.Generic;

namespace ParserFramework.Expression
{
    public class Parser
    {
        public static ParseRule NumberRule()
        {
            //number :: [Symbol(+ -)] NumberToken

            return new GroupRule
            {
                name = "number",
                kind = ParseRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    //new FunctionRule(list=> TokenParser.Symbol(list, "+", "-") ){ name = "signal", kind = ParseRule.Kind.Optional },
                    new TokenRule<NumberToken>("value")
                }
            };
        }

        public static ParseRule AdditionRule()
        {
            return new GroupRule()
            {
                name = "expr",
                kind = ParseRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    MultiplicationRule(),
                    new GroupRule()
                    {
                        name = "expr_op",
                        kind = ParseRule.Kind.Multiple,
                        rules = new List<ParseRule>()
                        {
                            //new TokenRule("op", TokenParser.Symbol, "+", "-"),
                            MultiplicationRule()
                        }
                    }
                }
            };
        }

        public static ParseRule MultiplicationRule()
        {
            return new GroupRule
            {
                name = "fator",
                kind = ParseRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    NumberRule(),
                    new GroupRule()
                    {
                        name = "fator_op",
                        kind = ParseRule.Kind.Multiple,
                        rules = new List<ParseRule>()
                        {
                            //new TokenRule("op", TokenParser.Symbol, "*", "/"),
                            NumberRule()
                        }
                    }
                }
            };
        }
    }
}
