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
            var chara = ' ';
            if (char.IsSymbol(CurrentChar) || char.IsPunctuation(CurrentChar))
            {
                chara = CurrentChar;
                Index += 1;
                return new SymbolToken(chara);
            }

            chara = CurrentChar;
            Index += 1;
            return Token.UNKNOWN;
        }
    }

}
