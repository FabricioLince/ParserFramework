using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework
{
    public abstract class ParseRule
    {
        public abstract ParsingInfo Execute(TokenList list);
        public string name;
    }
}
