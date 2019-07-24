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

            foreach (var rule in rules)
            {
                var info = rule.Execute(list);
                if (info == null) return null; // if it's null it's an error

                if (info.IsEmpty) continue; // if it's empty don't put it on the info

                if (info.info.Count == 1) // if only has one token child, put token child on info
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
            }

            return allInfo;
        }
    }
}
