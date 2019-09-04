using System;

namespace ParserFramework.Core.ParseRules
{
    public class FunctionRule : ParseRule
    {
        public Func<TokenList, ParsingInfo> parsingFunc;
        public FunctionRule(Func<TokenList, ParsingInfo> parsingFunc)
        {
            this.parsingFunc = parsingFunc;
        }
        protected override ParsingInfo Parse(TokenList list)
        {
            return parsingFunc(list);
        }
    }

    public class ChangeRuleKind : FunctionRule
    {
        public ChangeRuleKind(ParseRule rule, Kind newKind) : base(tokens => { rule.kind = newKind; return rule.Execute(tokens); })
        {
            name = rule.name;
        }
        public ChangeRuleKind(ParseRule rule, Kind newKind, string name) : base(tokens => { rule.kind = newKind; return rule.Execute(tokens); })
        {
            this.name = name;
        }
    }
}
