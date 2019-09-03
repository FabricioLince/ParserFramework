using System;

namespace ParserFramework.Examples.Script
{
    class ExpressionEvaluator
    {
        public static float Evaluate(Expression expr)
        {
            return Solve(expr);
        }

        static float Solve(Number number) { return number.value; }
         static float Solve(SubExpr subExpr)
        {
            var rt = Solve(subExpr.add);
            if (subExpr.signal != null && subExpr.signal == "-")
            {
                rt *= -1;
            }
            return rt;
        }
        static float Solve(Variable v)
        {
            if(Executor.variables.ContainsKey(v.varName))
            {
                return Executor.variables[v.varName];
            }
            Console.WriteLine(">! var '" + v.varName + "' yet to be initialized");
            return 0;
        }
        static float Solve(Term term)
        {
            if (term is Number n) return Solve(n);
            if (term is SubExpr s) return Solve(s);
            if (term is Variable v) return Solve(v);
            throw new Exception("Unkown Term");
        }
        static float Solve(Mult mult)
        {
            float result = Solve(mult.term);
            foreach (var multOp in mult.multOp)
            {
                switch (multOp.operatorSymbol)
                {
                    case "*":
                        result *= Solve(multOp.term);
                        break;
                    case "/":
                        result /= Solve(multOp.term);
                        break;
                }
            }
            return result;
        }
        static float Solve(Expression add)
        {
            float result = Solve(add.mult);
            foreach (var addOp in add.addOp)
            {
                switch (addOp.operatorSymbol)
                {
                    case "+":
                        result += Solve(addOp.mult);
                        break;
                    case "-":
                        result -= Solve(addOp.mult);
                        break;
                    default:
                        throw new Exception("Invalid Symbol for Add");
                }
            }
            return result;
        }
    }
}
