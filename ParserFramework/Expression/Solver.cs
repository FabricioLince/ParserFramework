using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework.Expression
{
    class Solver
    {
        public static bool TrySolve(string expression, out float result)
        {
            var list = Parser.DefaultTokenList(expression);
            var expr = Parser.AdditionRule.Execute(list);
            if (expr != null)
            {
                result = SolveAddition(expr);
                return true;
            }
            result = 0;
            return false;
        }

        static float SolveAddition(ParsingInfo expr)
        {
            if (expr.IsEmpty) return 0;
            if (expr.FirstInfo.name == "Add")
            {
                expr = expr.FirstInfo.AsChild;
            }
            float sum = 0;
            foreach (var pair in expr.info)
            {
                if (pair.Key == "Mult")
                {
                    sum = SolveMult(pair.Value.AsChild);
                }
                else if (pair.Key == "add_op")
                {
                    sum = SolveAddOperand_Multiple(sum, pair.Value.AsChild);
                }
                else
                {
                    Console.WriteLine("Add has \'" + pair.Key + "\'");
                }
            }

            return sum;
        }

        static float SolveAddOperand_Multiple(float sumSoFar, ParsingInfo info)
        {
            foreach (var pair in info.info)
            {
                sumSoFar = SolveAddOperand(sumSoFar, pair.Value.AsChild.FirstInfo.AsChild);
            }
            return sumSoFar;
        }
        static float SolveAddOperand(float sumSoFar, ParsingInfo info)
        {
            var opSymbolToken = info.GetToken("op") as SymbolToken;
            var op = opSymbolToken.Value;
            Console.WriteLine(op);

            float mult = SolveMult(info["Mult"].AsChild);

            switch(op)
            {
                case "+":
                    sumSoFar += mult;
                    break;
                case "-":
                    sumSoFar -= mult;
                    break;
            }

            foreach (var pair in info.info)
            {
                if (pair.Key == "op" || pair.Key == "Mult") { } else
                {
                    Console.WriteLine("add_op has \'" + pair.Key + "\'");
                }
            }
            return sumSoFar;
        }

        static float SolveMult(ParsingInfo info)
        {
            float product = 0;
            foreach (var pair in info.info)
            {
                if (pair.Key == "Term")
                {
                    product = SolveTerm(pair.Value.AsChild);
                }
                else if (pair.Key == "mult_op")
                {
                    product = SolveMultOperand_Multiple(product, pair.Value.AsChild);
                }
                else
                {
                    Console.WriteLine("Mult has \'" + pair.Key + "\'");
                }
            }
            Console.WriteLine("Mult solved to " + product);
            return product;
        }

        static float SolveMultOperand_Multiple(float sumSoFar, ParsingInfo info)
        {
            foreach (var pair in info.info)
            {
                sumSoFar = SolveMultOperand(sumSoFar, pair.Value.AsChild.FirstInfo.AsChild);
            }
            return sumSoFar;
        }
        static float SolveMultOperand(float productSoFar, ParsingInfo info)
        {
            var opSymbolToken = info.GetToken("op") as SymbolToken;
            var op = opSymbolToken.Value;
            Console.WriteLine(op);

            float term = SolveTerm(info["Term"].AsChild);

            switch (op)
            {
                case "*":
                    productSoFar *= term;
                    break;
                case "/":
                    productSoFar *= term;
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
            return productSoFar;
        }

        static float SolveTerm(ParsingInfo info)
        {
            float term = 0;
            foreach (var pair in info.info)
            {
                if (pair.Key == "Number")
                {
                    term = SolveNumber(pair.Value.AsChild);
                }
                else if (pair.Key == "sub_expr")
                {
                    term = SolveSubExpr(pair.Value.AsChild);
                }
                else
                {
                    Console.WriteLine("Term has \'" + pair.Key + "\'");
                }
            }
            Console.WriteLine("Term solved to " + term);
            return term;
        }

        public static float SolveSubExpr(ParsingInfo info)
        {
            return SolveAddition(info.FirstInfo.AsChild);

            foreach (var pair in info.info)
            {
                Console.WriteLine("sub_expr has \'" + pair.Key + "\'");
            }
            return 0;
        }

        public static float SolveNumber(ParsingInfo numberInfo)
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

            return value;
        }
    }
}
