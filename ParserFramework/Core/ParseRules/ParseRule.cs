namespace ParserFramework.ParseRules
{
    public abstract class ParseRule
    {
        public ParsingInfo Execute(TokenList list)
        {
            var info = Parse(list);
            if (info == null)
            {
                if (kind == Kind.Mandatory || kind == Kind.OneOrMore)
                    return null;
                return ParsingInfo.Empty;
            }

            if (kind == Kind.Multiple || kind == Kind.OneOrMore)
            {
                int childNo = 1;
                var allInfo = new ParsingInfo();
                allInfo.Add("child0", info);

                var otherInfo = Parse(list);
                while (otherInfo != null)
                {
                    allInfo.Add("child" + childNo, otherInfo);
                    childNo++;
                    otherInfo = Parse(list);
                }

                if (childNo > 1) return allInfo;
            }

            return info;
        }

        protected abstract ParsingInfo Parse(TokenList list);

        public string name;

        public Kind kind = Kind.Mandatory;
        public enum Kind
        {
            Mandatory, Optional, Multiple, OneOrMore
        }

    }
}
