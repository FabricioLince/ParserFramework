using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework
{
    public class ParsingInfo
    {
        public Dictionary<string, Token> tokens = new Dictionary<string, Token>();

        public override string ToString()
        {
            var rt = "info:\n";

            foreach (var pair in tokens)
            {
                if (pair.Value == null)
                    rt += pair.Key + ":null\n";
                else
                    rt += pair.Key + ":" + pair.Value.ToString() + "\n";
            }

            return rt;
        }
    }
}
