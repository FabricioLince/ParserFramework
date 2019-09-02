using ParserFramework.ParseRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Equation
{
    class Solver
    {
        internal static bool TrySolve(string input, out float result)
        {
            var equation = Parser.Parse(input);

            if (equation != null)
            {
                Console.WriteLine(equation);
                result = Solve(equation);
                return true;
            }

            result = 0;
            return false;
        }

        public static float Solve(Equation equation)
        {
            MixedTerm lhs = Solve(equation.lhs);
            MixedTerm rhs = Solve(equation.rhs);

            Console.WriteLine(lhs + " = " + rhs);

            float xTerms = lhs.xValue - rhs.xValue;
            float nTerms = rhs.pureValue - lhs.pureValue;

            Console.WriteLine(xTerms + "x = " + nTerms);
            return nTerms / xTerms;
        }

        public static MixedTerm Solve(XTerm xTerm) { return new MixedTerm(xTerm.value, 0); }
        public static MixedTerm Solve(Number number) { return new MixedTerm(0, number.value); }
        public static MixedTerm Solve(SubExpr subExpr)
        {
            var rt = Solve(subExpr.add);
            if (subExpr.signal != null && subExpr.signal == "-")
            {
                rt *= -1;
            }
            return rt;
        }
        public static MixedTerm Solve(Term term)
        {
            if (term is Number n) return Solve(n);
            if (term is XTerm x) return Solve(x);
            if (term is SubExpr s) return Solve(s);
            throw new Exception("Unkown Term");
        }
        public static MixedTerm Solve(Mult mult)
        {
            MixedTerm result = Solve(mult.term);
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
        public static MixedTerm Solve(Add add)
        {
            MixedTerm result = Solve(add.mult);
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

        public class MixedTerm
        {
            public float xValue;
            public float pureValue;
            public MixedTerm(float a = 0, float b = 0) { xValue = a; pureValue = b; }

            public static MixedTerm operator +(MixedTerm a, MixedTerm b)
            {
                return new MixedTerm()
                {
                    xValue = a.xValue + b.xValue,
                    pureValue = a.pureValue + b.pureValue
                };
            }
            public static MixedTerm operator -(MixedTerm a, MixedTerm b)
            {
                return new MixedTerm()
                {
                    xValue = a.xValue - b.xValue,
                    pureValue = a.pureValue - b.pureValue
                };
            }
            public static MixedTerm operator *(MixedTerm a, MixedTerm b)
            {
                if (a.xValue != 0 && b.xValue != 0)
                {
                    throw new Exception("Multiplication between two Xs (" + a + " * " + b + ")");
                }
                return new MixedTerm()
                {
                    xValue = a.xValue * b.pureValue + a.pureValue * b.xValue,
                    pureValue = a.pureValue * b.pureValue
                };
            }
            public static MixedTerm operator /(MixedTerm a, MixedTerm b)
            {
                MixedTerm ib = new MixedTerm(
                  b.xValue == 0 ? 0 : 1.0f / b.xValue,
                  b.pureValue == 0 ? 0 : 1.0f / b.pureValue);
                return a * ib;
            }
            public static MixedTerm operator *(float b, MixedTerm a) => a * b;
            public static MixedTerm operator *(MixedTerm a, float b)
            {
                return new MixedTerm()
                {
                    xValue = a.xValue * b,
                    pureValue = a.pureValue * b
                };
            }
            public static MixedTerm operator /(MixedTerm a, float b)
            {
                return new MixedTerm()
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
        }
    }
}
