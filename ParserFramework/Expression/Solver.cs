using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework.Expression
{
    class Solver
    {
        public static bool TrySolve(string expression, out float result)
        {
            Tokenizer tokenizer = new Tokenizer(new StringReader(expression));
            tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
            tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
            tokenizer.rules.Add(new Regex(@"^([a-z]+)"), m => new IdToken(m.Value));

            TokenList list = new TokenList(tokenizer);
            var expr = Parser.AdditionRule().Execute(list);
            if(expr != null)
            {
                result = SolveExpression(expr);
                return true;
            }
            result = 0;
            return false;
        }

        static float SolveExpression(ParsingInfo expr)
        {
            if (expr.IsEmpty) return 0;
            float sum = 0;
            foreach (var pair in expr.info)
            {
                if (pair.Key == "fator")
                {
                    sum = SolveFator(pair.Value as ParsingInfo.ChildInfo);
                }
                else if (pair.Key == "expr_op")
                {
                    sum = SolveExpressionOp(sum, pair.Value.AsChildInfo);
                }
                else
                {
                    Console.WriteLine("expr has " + pair.Key);
                }
            }

            return sum;
        }

        static float SolveExpressionOp(float currentValue, ParsingInfo.ChildInfo expr)
        {
            float sum = currentValue;
            foreach (var pair in expr.child.info)
            {
                if (pair.Key.StartsWith("child"))
                {
                    float value = 0;
                    string oper = "+";
                    foreach (var childPair in pair.Value.AsChildInfo.child.info)
                    {
                        if (childPair.Key == "op")
                        {
                            var opToken = childPair.Value.AsTokenInfo.token as SymbolToken;
                            oper = opToken.Value;
                        }
                        else if (childPair.Key == "fator")
                        {
                            value = SolveFator(childPair.Value.AsChildInfo);
                        }
                    }
                    switch (oper)
                    {
                        case "+":
                            sum += value;
                            break;
                        case "-":
                            sum -= value;
                            break;
                    }
                }
            }

            return sum;
        }

        static float SolveFator(ParsingInfo.ChildInfo groupInfo)
        {
            float product = 0;
            foreach (var pair in groupInfo.child.info)
            {
                if (pair.Key == "number")
                {
                    var number = SolveNumber(pair.Value as ParsingInfo.ChildInfo);
                    //Console.WriteLine("= " + number);
                    product = number;
                }
                else if (pair.Key == "fator_op")
                {
                    // fator_op has as many childs as there are operations
                    foreach (var fatorChildPair in pair.Value.AsChildInfo.child.info)
                    {
                        ParsingInfo fator_op = fatorChildPair.Value.AsChildInfo.child;
                        var opToken = fator_op.info["op"].AsTokenInfo.token as SymbolToken;
                        var number = SolveNumber(fator_op.info["number"].AsChildInfo);
                        //Console.WriteLine("fator op is " + opToken.Value + " " + number);
                        switch (opToken.Value)
                        {
                            case "*":
                                product *= number;
                                break;
                            case "/":
                                product /= number;
                                break;
                            default:
                                Console.WriteLine("Symbol " + opToken.Value + " not accepted");
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("fator has additional " + pair.Key);
                }
            }
            //Console.WriteLine("Result " + product);
            return product;
        }

        static float SolveNumber(ParsingInfo.ChildInfo numberInfo)
        {
            float value = 0;

            var numberToken = numberInfo.child.info["value"].AsTokenInfo.token;
            if (numberToken is IntToken)
            {
                value = (numberToken as IntToken).Value;
            }
            else if (numberToken is FloatToken)
            {
                value = (numberToken as FloatToken).Value;
            }

            if (numberInfo.child.info.ContainsKey("signal"))
            {
                var signalToken = numberInfo.child.info["signal"].AsTokenInfo.token;
                var symbol = signalToken as SymbolToken;
                if (symbol != null)
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
