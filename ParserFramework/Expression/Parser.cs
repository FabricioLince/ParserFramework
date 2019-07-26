using ParserFramework.Core;
using ParserFramework.ParseRules;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework.Expression
{
    public class Parser
    {
        //number :: [Symbol(+ -)] NumberToken
        public static ParseRule NumberRule => new GroupRule
        {
            name = "number",
            rules = new List<ParseRule>()
            {
                new SymbolRule("+", "-") { name = "signal", kind = ParseRule.Kind.Optional },
                new TokenRule<NumberToken>("value")
            }
        };

        public static ParseRule AdditionRule => new GroupRule()
        {
            name = "expr",
            kind = ParseRule.Kind.Mandatory,
            rules = new List<ParseRule>()
            {
                MultiplicationRule,
                new GroupRule()
                {
                    name = "expr_op",
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
            name = "fator",
            rules = new List<ParseRule>()
            {
                NumberRule,
                new GroupRule()
                {
                    name = "fator_op",
                    kind = ParseRule.Kind.Multiple,
                    //simplifyTree = true,
                    rules = new List<ParseRule>()
                    {
                        new SymbolRule("*", "/") { name = "op" },
                        NumberRule
                    }
                }
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
