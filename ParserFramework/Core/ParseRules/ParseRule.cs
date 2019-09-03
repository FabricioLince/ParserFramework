using System.Collections.Generic;

namespace ParserFramework.Core.ParseRules
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
        public List<ParseErrorInfo> LastErrors { get; private set; } = new List<ParseErrorInfo>();

        protected void AddChildErrors(List<ParseErrorInfo> childErrors)
        {
            errorInfo.AddRange(childErrors);
            foreach (var err in errorInfo)
            {
                if (err.rule == null) err.rule = this;
            }
            if (errorInfo.Count == 0) return;

            LastErrors.Clear();
            Token lastErrorGot = errorInfo[errorInfo.Count - 1].tokenGot;
            for (int i = 0; i < errorInfo.Count; i++)
            {
                if(errorInfo[i].tokenGot.Position == lastErrorGot.Position)
                {
                    if (LastErrors.Contains(errorInfo[i])) continue;
                    LastErrors.Add(errorInfo[i]);
                }
            }
        }

        /// <summary>
        /// being true means this will return ParsingInfo.Empty even if it's a match
        /// being false the ParsingInfo returned will contain every match
        /// </summary>
        public bool ignore = false;

        protected abstract ParsingInfo Parse(TokenList list);

        public ParsingInfo Execute(TokenList list)
        {
            errorInfo.Clear();
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
                return "expected " + expected + " | got " + got + position + "";
            return "expected " + expected + " | got " + got + position + " from " + rule.Descriptor + "";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ParseErrorInfo);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(ParseErrorInfo other)
        {
            return expected == other.expected
                && position == other.position;
        }
    }
}
