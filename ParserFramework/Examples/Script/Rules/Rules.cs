using ParserFramework.Core;
using ParserFramework.Core.ParseRules;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework.Examples.Script
{
    partial class Rules
    {
        public static ParseRule Command = new AlternateRule("Command")
        {
            possibilities = new List<ParseRule>()
            {
                AttribuitionCommand,
                PrintCommand,
            }
        };

        static ParseRule AttribuitionCommand => new AlternateRule("Attr")
        {
            possibilities = new List<ParseRule>()
            {
                ExpressionAttribution,
            }
        };

        static ParseRule ExpressionAttribution => new GroupRule("ExprAttr")
        {
            rules = new List<ParseRule>()
            {
                new IdRule() { name = "varName" },
                new SymbolRule("=") { ignore = true },
                Expression("expr"),
            }
        };

        static ParseRule PrintCommand => new GroupRule("Print")
        {
            rules = new List<ParseRule>()
            {
                new IdRule("print"){ ignore = true },
                PrintArg,
            }
        };
        static ParseRule PrintArg => new AlternateRule("arg")
        {
            kind = ParseRule.Kind.Multiple,
            possibilities = new List<ParseRule>()
            {
                Expression("expr"),
                new TokenRule<StringToken>("string"),
            }
        };

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
            tokenizer.rules.Add(new Regex(@"\'.*\'"), m => new StringToken(m.Value.Substring(1, m.Value.Length - 2)));
            tokenizer.rules.Add(new Regex("\".*\""), m => new StringToken(m.Value.Substring(1, m.Value.Length - 2)));

            tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
            tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
            tokenizer.rules.Add(new Regex(@"^(\w+)"), m => new IdToken(m.Value));

            


            TokenList list = new TokenList(tokenizer);
            return list;
        }
    }
}
