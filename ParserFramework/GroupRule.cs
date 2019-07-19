using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework
{
    public class GroupRule : ParseRule
    {
        public List<ParseRule> rules = new List<ParseRule>();
        public Kind kind = Kind.Mandatory;
        public enum Kind
        {
            Mandatory, Optional, Multiple, OneOrMore
        }

        public override ParsingInfo Execute(TokenList list)
        {            
            var allInfo = new ParsingInfo();

            var info = Parse(list);
            if(info == null)
            {
                if (kind == Kind.Mandatory || kind == Kind.OneOrMore)
                    return null;
                else if (kind == Kind.Optional || kind == Kind.Multiple)
                    return allInfo;
            }

            foreach (var pair in info.tokens)
            {
                allInfo.tokens.Add(pair.Key, pair.Value);
            }

            int label = 2;
            if (kind == Kind.Multiple || kind == Kind.OneOrMore)
            {
                info = Parse(list);
                while (info != null)
                {
                    foreach (var pair in info.tokens)
                    {
                        allInfo.tokens.Add(pair.Key + "_" + label, pair.Value);
                    }
                    label++;
                    info = Parse(list);
                }
            }

            return allInfo;
        }

        ParsingInfo Parse(TokenList list)
        {
            var allInfo = new ParsingInfo();
            int initialIndex = list.index;
            foreach (var rule in rules)
            {
                //Console.WriteLine("Executing " + rule.GetType().Name);
                var info = rule.Execute(list);
                if (info == null)
                {
                    list.index = initialIndex;
                    return null;
                }
                foreach (var pair in info.tokens)
                {
                    var key = pair.Key;
                    while (allInfo.tokens.ContainsKey(key))
                    {
                        key += "_";
                    }
                    allInfo.tokens.Add(key, pair.Value);
                }
            }
            return allInfo;
        }
    }
}
