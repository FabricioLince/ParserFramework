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
            var info = Parse(list);
            if (info == null)
            {
                if (kind == Kind.Mandatory || kind == Kind.OneOrMore)
                    return null;
                else if (kind == Kind.Optional || kind == Kind.Multiple)
                    return ParsingInfo.Empty;
            }

            if (kind == Kind.Mandatory || kind == Kind.Optional)
            {
                return info;
            }

            //(kind == Kind.Multiple || kind == Kind.OneOrMore)

            var allInfo = new ParsingInfo();
            int childNo = 0;
            allInfo.Add("child" + childNo, info);

            info = Parse(list);
            while (info != null)
            {
                childNo++;
                allInfo.Add("child" + childNo, info);

                info = Parse(list);
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

                if (info.IsEmpty) continue;

                if (info.info.Count == 1)
                {
                    var tokenInfo = info.info.First().Value.AsTokenInfo;
                    if (tokenInfo != null)
                    {
                        var tName = tokenInfo.name;
                        while (allInfo.info.ContainsKey(tName))
                        {
                            tName += "_";
                        }
                        allInfo.Add(tName, tokenInfo.token);
                        continue;
                    }
                }

                var name = rule.name;
                while(allInfo.info.ContainsKey(name))
                {
                    name += "_";
                }
                allInfo.Add(name, info);
                /*
                foreach (var pair in info.info)
                {
                    var key = pair.Key;
                    while (allInfo.info.ContainsKey(key))
                    {
                        key += "_";
                    }
                    allInfo.info.Add(key, pair.Value);
                }
                */
            }

            return allInfo;
        }
    }
}
