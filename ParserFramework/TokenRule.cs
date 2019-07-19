using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework
{
    public class TokenRule : ParseRule
    {
        public Func<TokenList, string[], Token> parsingFunc;
        public List<string> args = new List<string>();

        public TokenRule(string name, Func<TokenList, string[], Token> parsingFunc, params string[] args)
        {
            this.name = name;
            this.parsingFunc = parsingFunc;
            if (args != null && args.Length > 0)
                this.args.AddRange(args);
        }
        public TokenRule(Func<TokenList, string[], Token> parsingFunc, params string[] args)
        {
            this.name = "token";
            this.parsingFunc = parsingFunc;
            if (args != null && args.Length > 0)
                this.args.AddRange(args);
        }
        public override ParsingInfo Execute(TokenList list)
        {
            var token = parsingFunc(list, args.ToArray());
            if (token == null) return null;

            ParsingInfo info = new ParsingInfo();
            info.tokens.Add(name, token);
            return info;
        }
    }
}
