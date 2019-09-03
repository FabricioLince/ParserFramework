﻿using System;
using System.Collections.Generic;

namespace ParserFramework.Core.ParseRules
{
    class AlternateRule : ParseRule
    {
        public List<ParseRule> possibilities = new List<ParseRule>();

        protected override ParsingInfo Parse(TokenList list)
        {
            foreach (var rule in possibilities)
            {
                var info = rule.Execute(list);
                if (info != null)
                {
                    return info;
                }
                else
                {
                    AddChildErrors(rule.errorInfo);
                }
            }
            Error = true;
            return null;
        }
    }
}
