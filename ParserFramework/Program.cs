using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserFramework
{
    public class Tokenizer
    {
        readonly string buffer;
        int __index;
        public int Index
        {
            get { return __index; }
            set
            {
                __index = value;
                if (Index < buffer.Length)
                    Text = buffer.Substring(Index);
                else
                    Text = "";
            }
        }

        char CurrentChar => buffer[Index];
        public bool EOF => Index >= buffer.Length;
        public string Text { get; private set; }

        public Dictionary<Regex, Func<Match, Token>> rules = new Dictionary<Regex, Func<Match, Token>>();

        public Tokenizer(TextReader reader)
        {
            buffer = reader.ReadToEnd();
            Index = 0;
        }

        public Token NextToken()
        {
            while (!EOF && char.IsWhiteSpace(CurrentChar))
            {
                Index++;
            }
            if (EOF) return Token.EOF;

            Match bestMatch = null;
            Func<Match, Token> bestFunc = null;

            foreach (var rule in rules)
            {
                Regex regex = rule.Key;
                var m = regex.Match(Text);

                if (m.Success)
                {
                    if (bestMatch == null)
                    {
                        bestMatch = m;
                        bestFunc = rule.Value;
                    }
                    else if (m.Length > bestMatch.Length)
                    {
                        bestMatch = m;
                        bestFunc = rule.Value;
                    }
                }
            }

            if (bestMatch != null)
            {
                Index += bestMatch.Length;
                return bestFunc(bestMatch);
            }

            return Token.UNKNOWN;
        }
    }

    public class TokenList : List<Token>
    {
        List<Token> list => this;

        public int index = -1;

        public Token Current => index >= list.Count ? null : list[index];

        public bool MoveNext()
        {
            if (++index < list.Count && index >= 0)
                return list[index] != null;
            return false;
        }

        public Token Previous
        {
            get
            {
                if (--index < 0)
                    index = 0;
                return list[index];
            }
        }

        public TokenList(Tokenizer tokenizer)
        {
            var token = tokenizer.NextToken();
            while (token.kind != Token.Kind.EOF && !tokenizer.EOF)
            {
                list.Add(token);
                token = tokenizer.NextToken();
            }
            list.Add(token);
        }
    }

    public class Parser
    {
        
    }


    class Program
    {
        static void Main(string[] args)
        {
            string input = "+12-1";
            Tokenizer tokenizer = new Tokenizer(new StringReader(input));
            tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
            tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
            tokenizer.rules.Add(new Regex(@"^(\+|\-|\*|\/|\=)"), m => new SymbolToken(m.Value[0]));
            tokenizer.rules.Add(new Regex(@"^([a-z]+)"), m => new IdToken(m.Value));

            TokenList list = new TokenList(tokenizer);
            list.MoveNext();

            //addition :: number SymbolToken(+ -) number
            //number :: [SymbolToken(+ -)] NumberToken

            var expr = Addition(list);
            if (expr) Console.WriteLine("is expr");
            else Console.WriteLine("is not expr");

            foreach(Token token in list)
            {
                Console.WriteLine(token);
            }

            Console.ReadKey(true);
        }

        static bool Addition(TokenList list)
        {
            int initialIndex = list.index;

            if (!Number(list, out NumberToken numberA, out SymbolToken symbolA))
            {
                list.index = initialIndex;
                return false;
            }

            if (!Symbol(list, out SymbolToken op, "+", "-"))
            {
                list.index = initialIndex;
                return false;
            }

            if (!Number(list, out NumberToken numberB, out SymbolToken symbolB))
            {
                list.index = initialIndex;
                return false;
            }

            Console.WriteLine(numberA + " " + op + " " + numberB);

            return true;
        }

        static bool Number(TokenList list, out NumberToken number, out SymbolToken symbol)
        {
            number = null;
            int initialIndex = list.index;

            Symbol(list, out symbol, "+", "-");

            if (list.Current == null) return false;
            if (!(list.Current is NumberToken))
            {
                list.index = initialIndex;
                return false;
            }
            number = list.Current as NumberToken;
            list.MoveNext();

            return true;
        }

        static bool Symbol(TokenList list, out SymbolToken symbol, params string[] acceptedSymbols)
        {
            symbol = null;
            int initialIndex = list.index;

            if (list.Current == null) return false;

            if (!(list.Current is SymbolToken))
            {
                list.index = initialIndex;
                return false;
            }
            symbol = list.Current as SymbolToken;
            if (!acceptedSymbols.Contains(symbol.Value))
            {
                list.index = initialIndex;
                return false;
            }
            list.MoveNext();

            return true;
        }
    }
}
