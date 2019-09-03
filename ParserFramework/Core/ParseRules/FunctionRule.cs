using System;

namespace ParserFramework.Core.ParseRules
{
    public class FunctionRule : ParseRule
    {
        public Func<TokenList, ParsingInfo> parsingFunc;
        public FunctionRule(Func<TokenList, ParsingInfo> parsingFunc)
        {
            this.parsingFunc = parsingFunc;
        }
        protected override ParsingInfo Parse(TokenList list)
        {
            return parsingFunc(list);
        }
    }
}
