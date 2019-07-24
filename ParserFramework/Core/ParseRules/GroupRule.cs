using System.Collections.Generic;
using System.Linq;

namespace ParserFramework.ParseRules
{
    public class GroupRule : ParseRule
    {
        public List<ParseRule> rules = new List<ParseRule>();


        protected override ParsingInfo Parse(TokenList list)
        {
            var allInfo = new ParsingInfo();
            int initialIndex = list.index;
            foreach (var rule in rules)
            {
                //Console.WriteLine("Executing " + rule.GetType().Name);
                var info = rule.Execute(list);
                if (info == null)
                {
                    if (rule.kind == Kind.Mandatory || rule.kind == Kind.OneOrMore)
                    {
                        list.index = initialIndex;
                        return null;
                    }
                    else if (rule.kind == Kind.Optional || rule.kind == Kind.Multiple)
                        continue;
                }

                if (info.IsEmpty) continue;

                if (info.info.Count == 1 )//&& rule is TokenRule)
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
                while (allInfo.info.ContainsKey(name))
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
