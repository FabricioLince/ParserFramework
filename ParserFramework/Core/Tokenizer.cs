using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework.Core
{
    using TokenRule = Func<Match, Token>;
    using SpecialRule = Func<char, Token>;

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
        
        public Dictionary<Regex, TokenRule> rules = new Dictionary<Regex, TokenRule>();
        public Func<char, bool> ignore = c => false;
        List<SpecialRule> specialSymbolRules = new List<SpecialRule>();

        public enum PrioritizeMatchKind
        {
            First, Last, Longest, Shortest
        }
        public PrioritizeMatchKind prioritizeMethod = PrioritizeMatchKind.Longest;

        public Tokenizer(TextReader reader)
        {
            buffer = reader.ReadToEnd();
            Index = 0;
        }

        public Token NextToken()
        {
            while (!EOF && ignore(CurrentChar))
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
            TokenRule bestFunc = null;

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
                        if (prioritizeMethod == PrioritizeMatchKind.First) break;
                    }
                    else
                    {
                        switch (prioritizeMethod)
                        {
                            case PrioritizeMatchKind.First:
                                break;
                            case PrioritizeMatchKind.Last:
                                bestMatch = m;
                                bestFunc = rule.Value;
                                break;
                            case PrioritizeMatchKind.Longest:
                                if (m.Length > bestMatch.Length)
                                {
                                    bestMatch = m;
                                    bestFunc = rule.Value;
                                }
                                break;
                            case PrioritizeMatchKind.Shortest:
                                if (m.Length < bestMatch.Length)
                                {
                                    bestMatch = m;
                                    bestFunc = rule.Value;
                                }
                                break;
                        }
                    }
                }
            }

            if (bestMatch != null)
            {
                Index += bestMatch.Length;

                return Construct(bestFunc, bestMatch);
            }

            foreach(var symbolRule in specialSymbolRules)
            {
                var token = symbolRule(CurrentChar);
                if (token)
                {
                    var chara = CurrentChar;
                    Index += 1;

                    return Construct(token);
                }
            }

            Index += 1;

            return Construct(Token.UNKNOWN);
        }

        Token Construct(TokenRule rule, Match match)
        {
            var token = rule(match);
            return Construct(token, match.Length);
        }

        Token Construct(Token token, int skip = 1)
        {
            token.lineNumber = lineNumber;
            token.collumnNumber = collumnNumber;
            collumnNumber += skip;
            return token;
        }

        public void AddRegexRule(Regex regex, TokenRule rule)
        {
            rules.Add(regex, rule);
        }
        public void AddSpecialRule(Func<char, Token> rule)
        {
            specialSymbolRules.Add(rule);
        }
    }

}
