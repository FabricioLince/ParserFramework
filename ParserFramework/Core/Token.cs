namespace ParserFramework
{
    public class Token
    {
        public Kind kind { get; private set; }
        public int lineNumber, collumnNumber;
        public string Position => "[" + lineNumber + ":" + collumnNumber + "]";

        protected Token(Kind kind)
        {
            this.kind = kind;
        }

        public static readonly Token EOF = new Token(Kind.EOF);
        public static readonly Token UNKNOWN = new Token(Kind.UNKNOWN);

        public enum Kind
        {
            EOF,
            INT,
            FLOAT,
            SYMBOL,
            IDENTIFIER,
            CUSTOM,
            UNKNOWN
        }

        public override string ToString()
        {
            return "Token:" + kind;
        }

        public static implicit operator bool (Token t) => t != null;
    }

    public abstract class CustomToken : Token
    {
        protected CustomToken() : base(Kind.CUSTOM) { }
    }

    public abstract class NumberToken : Token
    {
        protected NumberToken(Kind k) : base(k) { }
        public float Value
        {
            get
            {
                if (this is IntToken i) return i.Value;
                return (this as FloatToken).Value;
            }
        }
    }

    public class IntToken : NumberToken
    {
        public new int Value { get; protected set; }
        public IntToken(int value) : base(Kind.INT)
        {
            this.Value = value;
        }
        public override string ToString()
        {
            return kind + " " + Value;
        }
    }
    public class FloatToken : NumberToken
    {
        public new float Value { get; protected set; }
        public FloatToken(float value) : base(Kind.FLOAT)
        {
            this.Value = value;
        }
        public override string ToString()
        {
            return kind + " " + Value;
        }
    }

    public class SymbolToken : Token
    {
        public string Value { get; protected set; }

        public SymbolToken(char symbol) : base(Token.Kind.SYMBOL)
        {
            Value = "" + symbol;
        }
        public SymbolToken(string symbol) : base(Token.Kind.SYMBOL)
        {
            Value = "" + symbol;
        }

        public override string ToString()
        {
            return kind + "" + Value;
        }
    }

    public class IdToken : Token
    {
        public string Value { get; private set; }
        public IdToken(string id) : base(Kind.IDENTIFIER)
        {
            this.Value = id;
        }
        public override string ToString()
        {
            return kind + " " + Value;
        }
    }
}
