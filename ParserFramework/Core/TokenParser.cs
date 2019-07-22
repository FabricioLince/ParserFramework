using System.Linq;

namespace ParserFramework.Core
{
    public static class TokenParser
    {
        public static NumberToken Number(TokenList list, params string[] args)
        {
            return list.Get<NumberToken>();
        }

        public static SymbolToken Symbol(TokenList list, params string[] acceptedSymbols)
        {
            var symbol = list.Get<SymbolToken>();
            if (symbol == null) return null;

            if (acceptedSymbols.Length > 0 && !acceptedSymbols.Contains(symbol.Value))
            {
                list.MoveToPrevious();
                return null;
            }

            return symbol;
        }

        public static IdToken Identifier(TokenList list, params string[] acceptedIds)
        {
            var id = list.Get<IdToken>();
            if (id == null) return null;

            if (acceptedIds.Length > 0 && !acceptedIds.Contains(id.Value))
            {
                list.MoveToPrevious();
                return null;
            }

            return id;
        }
    }
}
