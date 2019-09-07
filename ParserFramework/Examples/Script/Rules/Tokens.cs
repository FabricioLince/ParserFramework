using ParserFramework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserFramework.Examples.Script
{
    partial class Rules
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
                return "STRING \"" + Value + "\"";
            }
        }

        public class CommentToken : Token
        {
            public CommentToken() : base(Kind.CUSTOM) { }
            public override string ToString()
            {
                return "COMMENT";
            }
        }

        public static TokenList DefaultTokenList(string input)
        {
            Tokenizer tokenizer = new Tokenizer(new StringReader(input));

            tokenizer.AddRegexRule(new Regex("^\'([^\"\n])*\'"), m => new StringToken(m.Value.Substring(1, m.Value.Length - 2)));
            tokenizer.AddRegexRule(new Regex("^\"([^\"\n])*\""), m => new StringToken(m.Value.Substring(1, m.Value.Length - 2)));

            tokenizer.AddDefaultRuleForInteger();
            tokenizer.AddDefaultRuleForFloat();
            tokenizer.AddDefaultRuleForIdentifier();

            tokenizer.AddSymbolRule("==");
            tokenizer.AddSpecialRule(c =>
            {
                if (char.IsSymbol(c) || char.IsPunctuation(c))
                {
                    return new SymbolToken(c);
                }
                return null;
            });

            tokenizer.AddRegexRule(new Regex(@"^\/\/.*\n"), m => new CommentToken());

            tokenizer.ignore = c => char.IsWhiteSpace(c);

            TokenList list = new TokenList(tokenizer);
            return list;
        }
    }
}
