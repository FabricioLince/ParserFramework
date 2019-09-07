using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Core.ParseRules
{
    public class CheckPointRule : ParseRule
    {
        public CheckPointRule(string name)
        {
            this.name = name;
        }
        protected override ParsingInfo Parse(TokenList list)
        {
            checkPoint = this;
            return ParsingInfo.Empty;
        }
    }
}
