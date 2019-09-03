using System;

namespace ParserFramework.Core.ParseRules
{
    public class FunctionRule2 : ParseRule
    {
        public Func<TokenList, ParsingInfo> parsingFunc;
        public FunctionRule2(Func<TokenList, ParsingInfo> parsingFunc)
        {
            this.parsingFunc = parsingFunc;
        }
        protected override ParsingInfo Parse(TokenList list)
        {
            return parsingFunc(list);
        }
    }
}
