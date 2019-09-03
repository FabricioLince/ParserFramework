using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserFramework.Core.ParseRules
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
                    //Console.WriteLine(rule.errorInfo.ReduceToString("\n"));
                    AddChildErrors(rule.errorInfo);
                }

                if (info == null) // if it's null it's an error
                {
                    Error = true;
                    list.index = index;
                    //Console.WriteLine("Error on " + this.name + " " + this.errorInfo.Count);
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

                var realName = rule.name ?? "nameless " + rule.GetType().Name;
                var name = realName;

                while (allInfo.info.ContainsKey(name))
                {
                    name += "_";
                }

                if (rule is GroupRule)
                {
                    if (info.info.Count == 1)
                    {
                        if (realName == info.info.First().Key)
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
