using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework
{
    public class FunctionRule : ParseRule
    {
        public Func<TokenList, ParsingInfo> parsingFunc;
        public FunctionRule(Func<TokenList, ParsingInfo> parsingFunc)
        {
            this.parsingFunc = parsingFunc;
        }
        public override ParsingInfo Execute(TokenList list)
        {
            return parsingFunc(list);
        }
    }
}
