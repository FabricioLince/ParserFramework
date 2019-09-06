using System;

namespace ParserFramework.Examples.Script
{
    class ConditionEvaluator
    {
        public static bool Evaluate(Condition condition)
        {
            var lhs = ExpressionEvaluator.Evaluate(condition.expr);
            if (condition.comparation == null)
            {
                // no rhs on condition, evaluate lhs as bool
                return (lhs.Value != 0);
            }
            else
            {
                var rhs = ExpressionEvaluator.Evaluate(condition.comparation.expr);
                switch (condition.comparation.signal)
                {
                    case ">":
                        return (lhs.Value > rhs.Value);
                    case "<":
                        return(lhs.Value < rhs.Value);
                    case "==":
                        return(lhs.Value == rhs.Value);
                }
            }
            return false;
        }
    }

    class ExpressionEvaluator
    {
        public static Memory.Variable Evaluate(Expression expr)
        {
            return Solve(expr);
        }

        static Memory.Variable Solve(Number number)
        {
            return new Memory.Variable()
            {
                Value = number.value,
                type = number.type
            };
        }
        static Memory.Variable Solve(SubExpr subExpr)
        {
            var rt = Solve(subExpr.add);
            if (subExpr.signal != null && subExpr.signal == "-")
            {
                rt.Value *= -1;
            }
            return rt;
        }
        static Memory.Variable Solve(Variable v)
        {
            var mv = Memory.Get(v.varName);
            if (mv != null) return mv.Copy();

            Console.WriteLine(">! var '" + v.varName + "' yet to be initialized");
            return new Memory.Variable() { Value = 0 };
        }
        static Memory.Variable Solve(Term term)
        {
            if (term is Number n) return Solve(n);
            if (term is SubExpr s) return Solve(s);
            if (term is Variable v) return Solve(v);
            throw new Exception("Unkown Term");
        }
        static Memory.Variable Solve(Mult mult)
        {
            var result = Solve(mult.term);
            foreach (var multOp in mult.multOp)
            {
                var operand = Solve(multOp.term);
                switch (multOp.operatorSymbol)
                {
                    case "*":
                        result *= operand;
                        break;
                    case "/":
                        result /= operand;
                        break;
                    case "%":
                        result %= operand;
                        break;
                }
            }
            return result;
        }
        static Memory.Variable Solve(Expression add)
        {
            var result = Solve(add.mult);
            foreach (var addOp in add.addOp)
            {
                var operand = Solve(addOp.mult);
                switch (addOp.operatorSymbol)
                {
                    case "+":
                        result += operand;
                        break;
                    case "-":
                        result -= operand;
                        break;
                    default:
                        throw new Exception("Invalid Symbol for Add");
                }
            }
            return result;
        }
    }
}
