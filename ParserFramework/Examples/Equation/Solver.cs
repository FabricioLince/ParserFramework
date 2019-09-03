using System;

namespace ParserFramework.Examples.Equation
{
    class Solver
    {
        internal static bool TrySolve(string input, out float result, bool consumeWholeInput = true)
        {
            try
            {
                result = Solve(input, consumeWholeInput);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            catch(Rules.Exception e)
            {
                Console.WriteLine(e.Message);
            }

            result = 0;
            return false;
        }

        public static float Solve(string input, bool consumeWholeInput = true)
        {
            var equation = Parser.Parse(input, consumeWholeInput);

            if (equation != null)
            {
                //Console.WriteLine(equation);
                return Solve(equation);
            }

            throw new Rules.Exception(Rules.Equation);
        }

        public static float Solve(Equation equation)
        {
            Polinomial lhs = Solve(equation.lhs);
            Polinomial rhs = Solve(equation.rhs);

            //Console.WriteLine(lhs + " = " + rhs);

            float xTerms = lhs.xValue - rhs.xValue;
            float nTerms = rhs.pureValue - lhs.pureValue;

            if (xTerms == 0)
            {
                throw new Exception("No Xs left on equation");
            }

            //Console.WriteLine(xTerms + "x = " + nTerms);
            return nTerms / xTerms;
        }

        public static Polinomial Solve(XTerm xTerm) { return new Polinomial(xTerm.value, 0); }
        public static Polinomial Solve(Number number) { return new Polinomial(0, number.value); }
        public static Polinomial Solve(SubExpr subExpr)
        {
            var rt = Solve(subExpr.add);
            if (subExpr.signal != null && subExpr.signal == "-")
            {
                rt *= -1;
            }
            return rt;
        }
        public static Polinomial Solve(Term term)
        {
            if (term is Number n) return Solve(n);
            if (term is XTerm x) return Solve(x);
            if (term is SubExpr s) return Solve(s);
            throw new Exception("Unkown Term");
        }
        public static Polinomial Solve(Mult mult)
        {
            Polinomial result = Solve(mult.term);
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
        public static Polinomial Solve(Add add)
        {
            Polinomial result = Solve(add.mult);
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

        public class Polinomial
        {
            public float xValue;
            public float pureValue;
            public Polinomial(float a = 0, float b = 0) { xValue = a; pureValue = b; }

            public static Polinomial operator +(Polinomial a, Polinomial b)
            {
                return new Polinomial()
                {
                    xValue = a.xValue + b.xValue,
                    pureValue = a.pureValue + b.pureValue
                };
            }
            public static Polinomial operator -(Polinomial a, Polinomial b)
            {
                return new Polinomial()
                {
                    xValue = a.xValue - b.xValue,
                    pureValue = a.pureValue - b.pureValue
                };
            }
            public static Polinomial operator *(Polinomial a, Polinomial b)
            {
                if (a.xValue != 0 && b.xValue != 0)
                {
                    throw new Exception("Multiplication between two Xs (" + a + " * " + b + ")");
                }
                return new Polinomial()
                {
                    xValue = a.xValue * b.pureValue + a.pureValue * b.xValue,
                    pureValue = a.pureValue * b.pureValue
                };
            }
            public static Polinomial operator /(Polinomial a, Polinomial b)
            {
                Polinomial ib = new Polinomial(
                  b.xValue == 0 ? 0 : 1.0f / b.xValue,
                  b.pureValue == 0 ? 0 : 1.0f / b.pureValue);
                return a * ib;
            }
            public static Polinomial operator *(float b, Polinomial a) => a * b;
            public static Polinomial operator *(Polinomial a, float b)
            {
                return new Polinomial()
                {
                    xValue = a.xValue * b,
                    pureValue = a.pureValue * b
                };
            }
            public static Polinomial operator /(Polinomial a, float b)
            {
                return new Polinomial()
                {
                    xValue = a.xValue / b,
                    pureValue = a.pureValue / b
                };
            }

            public override string ToString()
            {
                if (xValue != 0 && pureValue != 0)
                    return ForceSignal(xValue) + "x " + ForceSignal(pureValue);
                if (xValue == 0)
                    return ForceSignal(pureValue);
                return ForceSignal(xValue) + "x";
            }
            string ForceSignal(float value)
            {
                return value < 0 ? value.ToString() : "+" + value;
            }

            public class Exception : Solver.Exception
            {
                public Exception(string message) : base(message) { }
            }
        }

        public class Exception : System.Exception
        {
            public Exception(string message) : base(message) { }
        }
    }
}
