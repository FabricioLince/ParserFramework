using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework
{
    public class Parser
    {
        public static ParseRule NumberRule()
        {
            //number :: [Symbol(+ -)] NumberToken

            return new GroupRule
            {
                name = "number",
                kind = ParseRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    new TokenRule(Symbol, "+", "-") { name = "signal" ,kind= ParseRule.Kind.Optional },
                    new TokenRule(Number) { name = "value" }
                }
            };
        }

        public static ParseRule AdditionRule()
        {
            return new GroupRule()
            {
                name = "expr",
                kind = ParseRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    MultiplicationRule(),
                    new GroupRule()
                    {
                        name = "expr_op",
                        kind = ParseRule.Kind.Multiple,
                        rules = new List<ParseRule>()
                        {
                            new TokenRule("op", Symbol, "+", "-"),
                            MultiplicationRule()
                        }
                    }
                }
            };
        }

        public static ParsingInfo String(TokenList list)
        {
            //string :: Symbol(") Identifier Symbol(")

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

        public static ParseRule MultiplicationRule()
        {
            return new GroupRule
            {
                name = "fator",
                kind = GroupRule.Kind.Mandatory,
                rules = new List<ParseRule>()
                {
                    NumberRule(),
                    new GroupRule()
                    {
                        name = "fator_op",
                        kind = GroupRule.Kind.Multiple,
                        rules = new List<ParseRule>()
                        {
                            new TokenRule("op", Symbol, "*", "/"),
                            NumberRule()
                        }
                    }
                }
            };
        }

        public static ParsingInfo Multiplication(TokenList list)
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
            info.info.Add("numberA", numberA.info["number"]);
            info.info.Add("symbolA", numberA.info["symbol"]);
            info.Add("operator", op);
            info.info.Add("numberB", numberB.info["number"]);
            info.info.Add("symbolB", numberB.info["symbol"]);
            return info;
        }

        public static ParsingInfo Addition(TokenList list)
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
            info.info.Add("numberA", numberA.info["number"]);
            info.info.Add("symbolA", numberA.info["symbol"]);
            info.Add("operator", op);
            info.info.Add("numberB", numberB.info["number"]);
            info.info.Add("symbolB", numberB.info["symbol"]);
            return info;
        }

        public static ParsingInfo Number(TokenList list)
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

            var rt = new ParsingInfo();
            rt.Add("number", number);
            rt.Add("symbol", symbol);
            return rt;
        }

        public static NumberToken Number(TokenList list, params string[] args)
        {
            if (list.Current == null) return null;
            if (!(list.Current is NumberToken))
            {
                return null;
            }
            var number = list.Current as NumberToken;
            list.MoveNext();
            return number;
        }

        public static SymbolToken Symbol(TokenList list, params string[] acceptedSymbols)
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

        public static IdToken Identifier(TokenList list, params string[] acceptedIds)
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
