using System;
using System.Collections.Generic;

namespace ParserFramework.Core.ParseRules
{
    class AlternateRule : ParseRule
    {
        public AlternateRule(string name = null) { this.name = name ?? "unnamed alt rule"; }

        public List<ParseRule> possibilities = new List<ParseRule>();

        protected override ParsingInfo Parse(TokenList list)
        {
            foreach (var rule in possibilities)
            {
                var info = rule.Execute(list);
                this.checkPoint = rule.checkPoint;

                if (info != null)
                {
                    return info;
                }
                else
                {
                    AddChildErrors(rule.errorInfo);
                    if (checkPoint != null)
                    {
                        //Console.WriteLine("hit check point " + checkPoint.name);
                        return null;
                    }
                }
            }
            Error = true;
            return null;
        }
    }
}
