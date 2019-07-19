﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework
{

    public class TokenList : List<Token>
    {
        List<Token> list => this;

        public int index = -1;

        public Token Current => index >= list.Count ? null : list[index];

        public bool MoveNext()
        {
            if (++index < list.Count && index >= 0)
                return list[index] != null;
            return false;
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
    }

}