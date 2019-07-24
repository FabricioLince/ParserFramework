using System;
using System.Collections.Generic;

namespace ParserFramework
{
    public class ParsingInfo
    {
        public static readonly ParsingInfo Empty = new ParsingInfo();
        public bool IsEmpty => info.Count == 0;

        public Dictionary<string, Info> info = new Dictionary<string, Info>();

        static int tab = 0;
        static string Tabs()
        {
            string rt = "";
            for (int i = 0; i < tab; i++)
            {
                rt += "\t";
            }
            return rt;
        }

        public void Add(string name, Token token)
        {
            info.Add(name, new TokenInfo() { name = name, token = token });
        }
        public void Add(string name, ParsingInfo child)
        {
            if (name == null) throw new ArgumentException("name can't be null");
            info.Add(name, new ChildInfo() { name = name, child = child });
        }

        public override string ToString()
        {
            var rt = "info:\n";

            foreach (var pair in info)
            {
                if (pair.Value == null)
                    rt += Tabs() + pair.Key + ":null\n";
                else
                    rt += Tabs() + pair.Key + ":" + pair.Value.ToString() + "\n";
            }

            return rt;
        }

        public abstract class Info
        {
            public string name;
            public TokenInfo AsTokenInfo => this as TokenInfo;
            public ChildInfo AsChildInfo => this as ChildInfo;
        }

        public class TokenInfo : Info
        {
            public Token token;
            public override string ToString()
            {
                return "TK:" + token;
            }
        }
        public class ChildInfo : Info
        {
            public ParsingInfo child;
            public override string ToString()
            {
                tab++;
                var rt = "child:" + child;
                tab--;
                return rt;
            }
        }
    }
}
