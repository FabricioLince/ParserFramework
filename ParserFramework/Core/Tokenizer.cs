using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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

        int lineNumber, collumnNumber = 0;

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
                collumnNumber++;
                if (CurrentChar == '\n')
                {
                    lineNumber++;
                    collumnNumber = 0;
                }

                Index++;
            }
            if (EOF) return Construct(Token.EOF);

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

                return Construct(bestFunc(bestMatch), bestMatch.Length);
            }
            var chara = ' ';
            if (char.IsSymbol(CurrentChar) || char.IsPunctuation(CurrentChar))
            {
                chara = CurrentChar;
                Index += 1;

                return Construct(new SymbolToken(chara));
            }

            chara = CurrentChar;
            Index += 1;

            return Construct(Token.UNKNOWN);
        }

        Token Construct(Token token, int skip = 1)
        {
            token.lineNumber = lineNumber;
            token.collumnNumber = collumnNumber;
            collumnNumber += skip;
            return token;
        }
    }

}
