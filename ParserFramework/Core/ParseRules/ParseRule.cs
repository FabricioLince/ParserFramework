namespace ParserFramework.ParseRules
{
    public abstract class ParseRule
    {
        public string name;

        public Kind kind = Kind.Mandatory;
        public enum Kind
        {
            Mandatory, Optional, Multiple, OneOrMore
        }

        /// <summary>
        /// being true means this will return ParsingInfo.Empty even if it's a match
        /// being false the ParsingInfo returned will contain every match
        /// </summary>
        public bool ignore = false;

        protected abstract ParsingInfo Parse(TokenList list);

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

                //if (childNo > 1)
                    return ignore ? ParsingInfo.Empty : allInfo;
            }

            return ignore ? ParsingInfo.Empty : info;
        }
    }
}
