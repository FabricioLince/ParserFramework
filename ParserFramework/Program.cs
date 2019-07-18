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
            string[] testCases = new string[]
            {
                "12+1",
                "1-1",
                "1*22",
                "\"test\"",
                "11",
                "2=2",
                "4/2"
            };

            foreach (string testCase in testCases)
            {
                Tokenizer tokenizer = new Tokenizer(new StringReader(testCase));
                tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
                tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
                tokenizer.rules.Add(new Regex(@"^(\+|\-|\*|\/|\=)"), m => new SymbolToken(m.Value[0]));
                tokenizer.rules.Add(new Regex(@"^([a-z]+)"), m => new IdToken(m.Value));

                TokenList list = new TokenList(tokenizer);
                list.MoveNext();

                Console.WriteLine("Testing: " + testCase);
                var info = Custom(list);
                if (info != null)
                {
                    Console.WriteLine("Is string");
                }
                var expr = Addition(list);
                if (expr != null)
                {
                    if ((expr.tokens["operator"] as SymbolToken).Value == "+")
                        Console.WriteLine("is addition");
                    else
                        Console.WriteLine("is subtraction");
                }
                else
                {
                    expr = Multiplication(list);
                    if (expr != null)
                    {
                        if ((expr.tokens["operator"] as SymbolToken).Value == "*")
                            Console.WriteLine("is multiplication");
                        else
                            Console.WriteLine("is division");
                    }
                    else
                    {
                        Console.WriteLine("is not expr");
                    }
                }

                //Console.WriteLine(expr);
                Console.WriteLine();
            }

            Console.ReadKey(true);
        }

        public class ParsingInfo
        {
            public Dictionary<string, Token> tokens = new Dictionary<string, Token>();

            public override string ToString()
            {
                var rt = "info:";

                foreach(var pair in tokens)
                {
                    if (pair.Value == null)
                        rt += pair.Key + ":null;";
                    else
                        rt += pair.Key + ":" + pair.Value.ToString() + "; ";
                }

                return rt;
            }
        }

        public abstract class ParseRule
        {
            public abstract ParsingInfo Execute(TokenList list);
            public string name;
        }
        public class GroupRule : ParseRule
        {
            public List<ParseRule> rules = new List<ParseRule>();
            public Kind kind = Kind.Mandatory;
            public enum Kind
            {
                Mandatory, Optional, Multiple, OneOrMore
            }

            public override ParsingInfo Execute(TokenList list)
            {
                int initialIndex = list.index;
                var allInfo = new ParsingInfo();
                foreach (var rule in rules)
                {
                    Console.WriteLine("Executing " + rule.GetType().Name);
                    var info = rule.Execute(list);
                    if (info == null)
                    {
                        if (kind == Kind.Mandatory)
                        {
                            list.index = initialIndex;
                            return null;
                        }
                    }
                    foreach (var pair in info.tokens)
                    {
                        var key = pair.Key;
                        if (allInfo.tokens.ContainsKey(key))
                        {
                            key += "_";
                        }
                        allInfo.tokens.Add(key, pair.Value);
                    }

                }

                return allInfo;
            }
        }
        public class TokenRule : ParseRule
        {
            public Func<TokenList, string[], Token> parsingFunc;
            public List<string> args = new List<string>();
            
            public TokenRule(string name, Func<TokenList, string[], Token> parsingFunc, params string[] args)
            {
                this.name = name;
                this.parsingFunc = parsingFunc;
                if (args != null && args.Length > 0)
                    this.args.AddRange(args);
            }
            public TokenRule(Func<TokenList, string[], Token> parsingFunc, params string[] args)
            {
                this.name = "token";
                this.parsingFunc = parsingFunc;
                if (args != null && args.Length > 0)
                    this.args.AddRange(args);
            }
            public override ParsingInfo Execute(TokenList list)
            {
                var token = parsingFunc(list, args.ToArray());
                if (token == null) return null;

                ParsingInfo info = new ParsingInfo();
                info.tokens.Add(name, token);
                return info;
            }
        }
        public class FunctionRule : ParseRule
        {
            public Func<TokenList, ParsingInfo> parsingFunc;
            public FunctionRule(Func<TokenList, ParsingInfo> parsingFunc)
            {
                this.parsingFunc = parsingFunc;
            }
            public override ParsingInfo Execute(TokenList list)
            {
                return parsingFunc(list);
            }
        }
        

        //number :: [Symbol(+ -)] NumberToken
        //string :: Symbol(") Identifier Symbol(")
        static ParsingInfo Custom(TokenList list)
        {
            //string[] ruleList = rules.Split('\n');

            GroupRule rule = new GroupRule
            {
                name = "string",
                kind = GroupRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    new TokenRule(Symbol, "\""),
                    new TokenRule(Identifier),
                    new TokenRule(Symbol, "\"")
                }
            };

            return rule.Execute(list);
        }

        static ParsingInfo Multiplication(TokenList list)
        {
            // mult :: number Symbol(* /) number
            int initialIndex = list.index;

            var numberA = Number(list);
            if (numberA == null)
            {
                list.index = initialIndex;
                return null;
            }

            var op = Symbol(list, "*", "/");
            if (op == null)
            {
                list.index = initialIndex;
                return null;
            }

            var numberB = Number(list);
            if (numberB == null)
            {
                list.index = initialIndex;
                return null;
            }

            ParsingInfo info = new ParsingInfo();
            info.tokens.Add("numberA", numberA.tokens["number"]);
            info.tokens.Add("symbolA", numberA.tokens["symbol"]);
            info.tokens.Add("operator", op);
            info.tokens.Add("numberB", numberB.tokens["number"]);
            info.tokens.Add("symbolB", numberB.tokens["symbol"]);
            return info;
        }

        static ParsingInfo Addition(TokenList list)
        {
            //addition :: number SymbolToken(+ -) number
            int initialIndex = list.index;

            var numberA = Number(list);
            if (numberA == null)
            {
                list.index = initialIndex;
                return null;
            }

            var op = Symbol(list, "+", "-");
            if(op == null)
            {
                list.index = initialIndex;
                return null;
            }

            var numberB = Number(list);
            if (numberB == null)
            {
                list.index = initialIndex;
                return null;
            }

            ParsingInfo info = new ParsingInfo();
            info.tokens.Add("numberA", numberA.tokens["number"]);
            info.tokens.Add("symbolA", numberA.tokens["symbol"]);
            info.tokens.Add("operator", op);
            info.tokens.Add("numberB", numberB.tokens["number"]);
            info.tokens.Add("symbolB", numberB.tokens["symbol"]);
            return info;
        }

        static ParsingInfo Number(TokenList list)
        {
            //number :: [SymbolToken(+ -)] NumberToken
            int initialIndex = list.index;

            var symbol = Symbol(list, "+", "-");

            if (list.Current == null) return null;
            if (!(list.Current is NumberToken))
            {
                list.index = initialIndex;
                return null;
            }
            var number = list.Current as NumberToken;
            list.MoveNext();

            return new ParsingInfo()
            {
                tokens = new Dictionary<string, Token>()
                {
                    { "number", number },
                    { "symbol", symbol }
                }
            };
        }

        static SymbolToken Symbol(TokenList list, params string[] acceptedSymbols)
        {
            int initialIndex = list.index;

            if (list.Current == null) return null;

            if (!(list.Current is SymbolToken))
            {
                list.index = initialIndex;
                return null;
            }
            var symbol = list.Current as SymbolToken;
            if (acceptedSymbols.Length > 0 && !acceptedSymbols.Contains(symbol.Value))
            {
                list.index = initialIndex;
                return null;
            }
            list.MoveNext();

            return symbol;
        }

        static IdToken Identifier(TokenList list, params string[] acceptedIds)
        {
            int initialIndex = list.index;

            if (list.Current == null) return null;

            if (!(list.Current is IdToken))
            {
                list.index = initialIndex;
                return null;
            }
            var id = list.Current as IdToken;
            if (acceptedIds.Length > 0 && !acceptedIds.Contains(id.Value))
            {
                list.index = initialIndex;
                return null;
            }
            list.MoveNext();

            return id;
        }
    }
}
/*
 * 
            rule.name = "number";
            rule.groups.Add(new ParseRule.GroupRule()
            {
                kind = ParseRule.GroupRule.Kind.Optional,
                tokens = new List<ParseRule.TokenRule>()
                {
                    new ParseRule.TokenRule(Symbol, "+", "-")
                }
            });
            rule.groups.Add(new ParseRule.GroupRule()
            {
                kind = ParseRule.GroupRule.Kind.Mandatory,
                tokens = new List<ParseRule.TokenRule>()
                {
                    new ParseRule.TokenRule(Number)
                }
            });
*/