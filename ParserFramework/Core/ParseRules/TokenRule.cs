using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserFramework.ParseRules
{
    public class TokenRule<TokenType> : ParseRule where TokenType:Token
    {
        Predicate<TokenType> filter;

        public TokenRule() { name = "token_" + typeof(TokenType).Name; }
        public TokenRule(string name)
        {
            this.name = name;
        }

        public TokenRule(Predicate<TokenType> filter)
        {
            this.filter = filter;
        }

        protected override ParsingInfo Parse(TokenList list)
        {
            var token = list.Get<TokenType>();
            if (token == null) return null;
            if (filter != null && filter(token) == false)
            {
                return null;
            }

            ParsingInfo info = new ParsingInfo();
            info.Add(name, token);
            return info;
        }
    }

    public class SymbolRule : TokenRule<SymbolToken>
    {
        public SymbolRule(params string[] acceptedSymbols)
            : base(token => acceptedSymbols.Length == 0 || acceptedSymbols.Contains(token.Value))
        {
        }
    }

    public class IdRule : TokenRule<IdToken>
    {
        public IdRule(params string[] acceptedSymbols)
            : base(token => acceptedSymbols.Length == 0 || acceptedSymbols.Contains(token.Value))
        {
        }
    }
}
