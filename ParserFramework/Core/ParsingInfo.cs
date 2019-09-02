using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ParserFramework
{
    public class ParsingInfo : IEnumerable<KeyValuePair<string, ParsingInfo.Info>>
    {
        public static readonly ParsingInfo Empty = new ParsingInfo();
        public bool IsEmpty => info.Count == 0;

        public Dictionary<string, Info> info = new Dictionary<string, Info>();

        static int tab = 0;
        static string Tabs()
        {
            return "    ".Repeat(tab);
        }

        public TokenInfo Add(string name, Token token)
        {
            if (name == null) throw new ArgumentException("name can't be null");
            var tokenInfo = new TokenInfo() { name = name, token = token };
            info.Add(name, tokenInfo);
            return tokenInfo;
        }
        public ChildInfo Add(string name, ParsingInfo child)
        {
            if (name == null) throw new ArgumentException("name can't be null");
            var childInfo = new ChildInfo() { name = name, child = child };
            info.Add(name, childInfo);
            return childInfo;
        }

        public Token GetToken(string name)
        {
            if (info.ContainsKey(name) == false) return null;
            var i = info[name];
            return i.AsToken;
        }

        public Info FirstInfo
        {
            get
            {
                return info.First().Value;
            }
        }

        public Info this[string name]
        {
            get { return info[name]; }
            set { info[name] = value; }
        }
        public Info Get(string name) { return info.ContainsKey(name) ? info[name] : null; }

        public override string ToString()
        {
            if (IsEmpty) return "info:empty\n";

            var rt = "info:\n";

            foreach (var pair in info)
            {
                if (pair.Value == null)
                    rt += Tabs() + pair.Key + ":null\n";
                else
                    rt += Tabs() + pair.Key + ":" + pair.Value.ToString() + "\n";
            }

            return rt.Substring(0, rt.Length - 1); // remove last '\n'
        }

        public IEnumerator<KeyValuePair<string, Info>> GetEnumerator()
        {
            return info.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return info.GetEnumerator();
        }

        public abstract class Info
        {
            public string name;
            public TokenInfo AsTokenInfo => this as TokenInfo;
            public ChildInfo AsChildInfo => this as ChildInfo;

            public Token AsToken => AsTokenInfo?.token ?? null;
            public ParsingInfo AsChild => AsChildInfo?.child ?? null;
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
