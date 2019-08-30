using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserFramework.ParseRules
{
    public class TokenRule<TokenType> : ParseRule where TokenType:Token
    {
        public Predicate<TokenType> filter;

        public TokenRule() { name = "token_" + typeof(TokenType).Name; }
        public TokenRule(string name)
        {
            this.name = name;
        }

        public TokenRule(Predicate<TokenType> filter)
        {
            name = "token_" + typeof(TokenType).Name;
            this.filter = filter;
        }

        protected override ParsingInfo Parse(TokenList list)
        {
            var token = list.Get<TokenType>();
            if (token == null)
            {
                errorInfo.Add(new ParseErrorInfo()
                {
                    expected = (typeof(TokenType).Name),
                    got = (list.Current.ToString()),
                    tokenGot = list.Current,
                });
                Error = true;
                return null;
            }

            if (filter != null && filter(token) == false)
            {
                list.MoveToPrevious();
                errorInfo.Add(new ParseErrorInfo()
                {
                    expected = (typeof(TokenType).Name),
                    got = (list.Current.ToString() + ", but filter failed"),
                    tokenGot = list.Current,
                });
                Error = true;
                return null;
            }

            ParsingInfo info = new ParsingInfo();
            info.Add(name, token);
            return info;
        }
    }

    public class NumberRule : TokenRule<NumberToken>
    {
    }

    public class SymbolRule : TokenRule<SymbolToken>
    {
        List<string> acceptedSymbols;
        public SymbolRule(params string[] acceptedSymbols)
            : base(token => acceptedSymbols.Length == 0 || acceptedSymbols.Contains(token.Value))
        {
            this.acceptedSymbols = new List<string>(acceptedSymbols);
        }

        protected override ParsingInfo Parse(TokenList list)
        {
            var info = base.Parse(list);

            if (info == null)
            {
                errorInfo.Clear();
                errorInfo.Add(new ParseErrorInfo()
                {
                    expected = ("Symbol " + acceptedSymbols.ReduceToString(" ")),
                    got = (list.Current.ToString()),
                    tokenGot = list.Current,
                });
            }

            return info;
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
