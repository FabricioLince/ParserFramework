using ParserFramework.ParseRules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Equation
{
    class Expander
    {
        public static string Expand(string input)
        {
            TokenList list = Expression.Parser.DefaultTokenList(input);
            var rule = Parser.Main;

            var info = rule.Execute(list);
            if (info == null) Console.WriteLine("not an " + rule.name);

            string lhs = ExpandAddition(info.FirstInfo.AsChild);
            //Console.WriteLine(lhs);
            lhs = lhs.Replace("--", "+");
            lhs = lhs.Replace("-+", "-");
            //Console.WriteLine(lhs);

            string rhs = ExpandAddition(info.FirstInfo.AsChild["Add_"].AsChild);
            //Console.WriteLine(rhs);
            rhs = rhs.Replace("--", "+");
            rhs = rhs.Replace("-+", "-");
            //Console.WriteLine(rhs);
            return lhs + " = " + rhs;
        }

        static string ExpandAddition(ParsingInfo expr)
        {
            if (expr.FirstInfo.name == "Add")
            {
                expr = expr.FirstInfo.AsChild;
            }
            string str = "";
            foreach (var pair in expr.info)
            {
                if (pair.Key == "Mult")
                {
                    str = ExpandMult(pair.Value.AsChild);
                }
                else if (pair.Key == "add_op")
                {
                    str += ExpandAddOperand_Multiple(pair.Value.AsChild);
                }
                else
                {
                    Console.WriteLine("Add has \'" + pair.Key + "\'");
                }
            }

            return str;
        }

        static string ExpandAddOperand_Multiple(ParsingInfo info)
        {
            string op = "";
            foreach (var pair in info.info)
            {
                op += SolveAddOperand(pair.Value.AsChild);
            }
            return op;
        }
        static string SolveAddOperand(ParsingInfo info)
        {
            string str = "";
            var opSymbolToken = info.GetToken("op") as SymbolToken;
            var op = opSymbolToken.Value;
            //Console.WriteLine(op);

            string mult = ExpandMult(info["Mult"].AsChild);
            
            switch (op)
            {
                case "+":
                    str = " + " + mult;
                    break;
                case "-":
                    str = " + -" + mult;
                    break;
            }

            foreach (var pair in info.info)
            {
                if (pair.Key == "op" || pair.Key == "Mult") { }
                else
                {
                    Console.WriteLine("add_op has \'" + pair.Key + "\'");
                }
            }
            return str;
        }

        static string ExpandMult(ParsingInfo info)
        {
            string str = "";
            foreach (var pair in info.info)
            {
                if (pair.Key == "Term")
                {
                    str = SolveTerm(pair.Value.AsChild);
                }
                else if (pair.Key == "mult_op")
                {
                    str += SolveMultOperand_Multiple(pair.Value.AsChild);
                }
                else
                {
                    Console.WriteLine("Mult has \'" + pair.Key + "\'");
                }
            }
            //Console.WriteLine("Mult solved to " + product);
            return str;
        }

        static string SolveMultOperand_Multiple(ParsingInfo info)
        {
            string str = "";
            foreach (var pair in info.info)
            {
                str+= SolveMultOperand( pair.Value.AsChild);
            }
            return str;
        }
        static string SolveMultOperand(ParsingInfo info)
        {
            string str = "";
            var opSymbolToken = info.GetToken("op") as SymbolToken;
            var op = opSymbolToken.Value;
            //Console.WriteLine(op);

            string term = SolveTerm(info["Term"].AsChild);

            switch (op)
            {
                case "*":
                    str = " * " + term;
                    break;
                case "/":
                    str = " / " + term;
                    break;
            }

            foreach (var pair in info.info)
            {
                if (pair.Key == "op" || pair.Key == "Term") { }
                else
                {
                    Console.WriteLine("mult_op has \'" + pair.Key + "\'");
                }
            }
            return str;
        }

        static string SolveTerm(ParsingInfo info)
        {
            foreach (var pair in info.info)
            {
                if (pair.Key == "Number")
                {
                    return ExpandNumber(pair.Value.AsChild);
                }
                else if (pair.Key == "sub_expr")
                {
                    return SolveSubExpr(pair.Value.AsChild);
                }
                else if (pair.Key == "XTerm")
                {
                    return ExpandXTerm(pair.Value.AsChild);
                }
                else
                {
                    Console.WriteLine("Term has \'" + pair.Key + "\'");
                }
            }
            //Console.WriteLine("Term solved to " + term);
            return " ?term ";
        }

        public static string SolveSubExpr(ParsingInfo info)
        {
            return " ( " + ExpandAddition(info.FirstInfo.AsChild) + " ) ";
        }

        public static string ExpandNumber(ParsingInfo numberInfo)
        {
            float value = 0;

            var numberToken = numberInfo["value"].AsToken;
            if (numberToken is IntToken integer)
            {
                value = integer.Value;
            }
            else if (numberToken is FloatToken floating)
            {
                value = floating.Value;
            }

            if (numberInfo.info.ContainsKey("signal"))
            {
                if (numberInfo["signal"].AsToken is SymbolToken symbol)
                {
                    if (symbol.Value == "-")
                    {
                        value *= -1;
                    }
                }
            }

            return value < 0 ? value.ToString() : "" + value;
        }

        static string ExpandXTerm(ParsingInfo info)
        {
            ParsingInfo numberInfo = null;
            SymbolToken signalToken = null;

            foreach (var pair in info.info)
            {
                if (pair.Key == "Number")
                {
                    numberInfo = pair.Value.AsChild;
                }
                else if (pair.Key == "signal")
                {
                    signalToken = pair.Value.AsToken as SymbolToken;
                    
                    Console.WriteLine("Signal => " + signalToken);
                    if (signalToken.Value == "+") signalToken = null;
                }
                else if (pair.Key == "var")
                { }
                else Console.WriteLine("XTerm has " + pair.Key);
            }

            if (numberInfo != null)
            {
                return ExpandNumber(numberInfo) + "x";
            }
            else if (signalToken != null)
            {
                return signalToken.Value + "1x";
            }

            return "1x";
        }


        static ParseRule ExpandRuleNX => new GroupRule
        {
            name = "nx",
            rules = new List<ParseRule>()
            {
                new NumberRule() { name = "n" },
                new SymbolRule("-"),
                new IdRule("x") { name = "x" }
            }
        };
        static ParseRule ExpandRuleXX => new GroupRule
        {
            name = "xx",
            rules = new List<ParseRule>()
            {
                new IdRule("x") { name = "x" },
                new SymbolRule("-"),
                new IdRule("x") { name = "x" },
            }
        };
        static ParseRule ExpandRuleXN => new GroupRule
        {
            name = "xn",
            rules = new List<ParseRule>()
            {
                new IdRule("x") { name = "x" },
                new SymbolRule("-"),
                new NumberRule() { name = "n" },
            }
        };
        static ParseRule ExpandRuleNN => new GroupRule
        {
            name = "nn",
            rules = new List<ParseRule>()
            {
                new NumberRule() { name = "n" },
                new SymbolRule("-"),
                new NumberRule() { name = "n" },
            }
        };
    }
}
