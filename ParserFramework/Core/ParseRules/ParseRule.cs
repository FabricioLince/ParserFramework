using System.Collections.Generic;

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

        public string Descriptor => GetType().Name + " " + name;

        public List<ParseErrorInfo> errorInfo = new List<ParseErrorInfo>();
        public bool Error { get; protected set; } = false;

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
                var child = allInfo.Add("child0", new ParsingInfo());

                foreach (var c in info.FirstInfo.AsChild.info)
                {
                    if(c.Value.AsChild != null)
                        child.AsChild.Add(c.Key, c.Value.AsChild);
                    else
                        child.AsChild.Add(c.Key, c.Value.AsToken);
                }

                var otherInfo = Parse(list);
                while (otherInfo != null)
                {
                    child = allInfo.Add("child" + childNo, new ParsingInfo());

                    foreach (var c in otherInfo.FirstInfo.AsChild.info)
                    {
                        if (c.Value.AsChild != null)
                            child.AsChild.Add(c.Key, c.Value.AsChild);
                        else
                            child.AsChild.Add(c.Key, c.Value.AsToken);
                    }

                    childNo++;
                    otherInfo = Parse(list);
                }

                //if (childNo > 1)
                    return ignore ? ParsingInfo.Empty : allInfo;
            }

            return ignore ? ParsingInfo.Empty : info;
        }
    }

    public class ParseErrorInfo
    {
        public string expected;
        public string got;
        public Token tokenGot;
        string position => tokenGot == null ? "" : " " + tokenGot.Position;
        public ParseRule rule;

        public override string ToString()
        {
            if (rule == null)
                return "expected " + expected + " | got " + got + position + "\n";
            return "expected " + expected + " | got " + got + position + " from rule " + rule.Descriptor + "\n";
        }
    }
}
