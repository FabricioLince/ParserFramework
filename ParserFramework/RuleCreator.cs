using ParserFramework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserFramework
{
    class RuleCreator
    {
        public static ParseRule Create(string input)
        {

            return null;
        }

        static ParsingInfo ParseRuleString(string input)
        {
            var list = GetTokens(input);
            return RuleStringRule().Execute(list);
        }

        static ParseRule RuleStringRule()
        {
            return new GroupRule
            {
                name = "RuleString",
                kind = ParseRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    new TokenRule("name", Core.TokenParser.Identifier),
                    new TokenRule("::", Core.TokenParser.Symbol, "::"),
                    Anything()
                }
            };
        }
        static ParseRule Anything()
        {
            return new GroupRule
            {
                name = "thing",
                kind = ParseRule.Kind.Multiple,
                rules = new List<ParseRule>()
                {
                    new TokenRule("id", TokenParser.Identifier){ kind = ParseRule.Kind.Optional },
                    new TokenRule("sb", TokenParser.Symbol){ kind = ParseRule.Kind.Optional },
                }
            };
        }

        public static TokenList GetTokens(string input)
        {
            Tokenizer tokenizer = new Tokenizer(new StringReader(input));
            tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
            tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
            tokenizer.rules.Add(new Regex(@"^(\w+)"), m => new IdToken(m.Value));
            tokenizer.rules.Add(Program.RegexForSymbols("::"), m => new SymbolToken(m.Value));

            TokenList list = new TokenList(tokenizer);
            return list;
        }
    }
}
