using System.Collections.Generic;

namespace ParserFramework
{

    public class TokenList : List<Token>
    {
        List<Token> list => this;

        public int index = -1;

        public Token Current => index < 0 ? list[index = 0] : index >= list.Count ? null : list[index];

        public bool MoveNext()
        {
            if (++index < list.Count && index >= 0)
                return list[index] != null;
            return false;
        }
        public bool MoveToPrevious()
        {
            return index > 0 && Previous != null;
        }

        public Token Previous
        {
            get
            {
                if (--index < 0)
                    index = 0;
                return list[index];
            }
        }

        public TokenList(Tokenizer tokenizer)
        {
            var token = tokenizer.NextToken();
            while (token.kind != Token.Kind.EOF && !tokenizer.EOF)
            {
                //Console.WriteLine("Adding " + token);
                list.Add(token);
                token = tokenizer.NextToken();
            }
            //Console.WriteLine("Adding " + token);
            list.Add(token);
        }

        public TokenT Get<TokenT>() where TokenT : Token
        {
            if (Current == null) return null;
            if (!(Current is TokenT))
            {
                return null;
            }
            var rt = Current as TokenT;
            MoveNext();
            return rt;
        }
    }

}
