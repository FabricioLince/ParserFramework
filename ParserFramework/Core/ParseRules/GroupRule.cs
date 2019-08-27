using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserFramework.ParseRules
{
    public class GroupRule : ParseRule
    {
        public List<ParseRule> rules
        {
            set
            {
                rulesF = new List<Func<ParseRule>>();
                foreach(var rule in value)
                {
                    rulesF.Add(() => rule);
                }
            }
        }

        public List<Func<ParseRule>> rulesF = new List<Func<ParseRule>>();

        protected override ParsingInfo Parse(TokenList list)
        {
            var allInfo = new ParsingInfo();

            foreach (var ruleF in rulesF)
            {
                var rule = ruleF();
                var info = rule.Execute(list);
                if (info == null) return null; // if it's null it's an error

                if (info.IsEmpty) continue; // if it's empty don't put it on the info

                // if only has one token child, put token child on info
                if (!(rule is GroupRule) && info.info.Count == 1)
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

                var name = rule.name??"nameless " +rule.GetType().Name;
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
