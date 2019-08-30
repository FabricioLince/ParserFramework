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
            var rt = new ParsingInfo();
            var allInfo = new ParsingInfo();
            rt.Add(this.name, allInfo);
            int index = list.index;

            foreach (var ruleF in rulesF)
            {
                var rule = ruleF();
                var info = rule.Execute(list);

                //if(rule.kind != Kind.Optional)
                {
                    errorInfo.AddRange(rule.errorInfo);
                    foreach (var err in errorInfo)
                    {
                        if (err.rule == null) err.rule = this;
                    }
                }

                if (info == null) // if it's null it's an error
                {
                    Error = true;
                    if (!allInfo.IsEmpty)
                    {
                    }
                    list.index = index;
                    return null; 
                }

                if (info.IsEmpty) continue; // if it's empty don't put it on the info

                /**/
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
                /**/

                var name = rule.name ?? "nameless " + rule.GetType().Name;

                while (allInfo.info.ContainsKey(name))
                {
                    name += "_";
                }

                if (rule is GroupRule)
                {
                    if (info.info.Count == 1)
                    {
                        if (name == info.info.First().Key)
                        {
                            allInfo.Add(name, info.info.First().Value.AsChild);
                            continue;
                        }
                    }

                }

                allInfo.Add(name, info);
            }

            return rt;
        }
    }
}
