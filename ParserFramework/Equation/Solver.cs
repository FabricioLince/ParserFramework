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
            var list = Parser.DefaultTokenList(input);
            var expr = Parser.Main.Execute(list);
            if (expr != null)
            {
                result = Solve(expr);
                return true;
            }
            result = 0;
            return false;
        }

        class Term
        {
            public float value = 0;
            public enum Side { Left, Right }
        }
        static List<Term> terms = new List<Term>();
        static List<Term> xTerms = new List<Term>();

        static float Solve(ParsingInfo info)
        {
            if (info.FirstInfo.name == "Equation")
            {
                SolveEquation(info.FirstInfo.AsChild);
            }
            return 0;
        }
        static void SolveEquation(ParsingInfo info)
        {
            terms.Clear();
            xTerms.Clear();
            foreach (var pair in info.info)
            {
                if(pair.Key=="Add")
                    AddTermsOf(pair.Value.AsChild, Term.Side.Left);
                else
                    AddTermsOf(pair.Value.AsChild, Term.Side.Right);
            }
            Console.WriteLine(xTerms.ReduceToString(t=>t.value.ToString(), "\n"));
            float lhs = 0;
            foreach(var term in xTerms)
            {
                lhs += term.value;
            }
            float rhs = 0;
            foreach (var term in terms)
            {
                rhs += term.value;
            }
            Console.WriteLine(lhs + "x = " + rhs);
            float x = rhs / lhs;
            Console.WriteLine("x = " + x);

        }

        private static void AddTermsOf(ParsingInfo info, Term.Side side)
        {
            if (info == null) return;
            foreach (var pair in info.info)
            {
                //Console.WriteLine("Adding has \'" + pair.Key + "\'");

                if (pair.Key == "Term")
                {
                    var termInfo = pair.Value.AsChild.FirstInfo.AsChild;
                    //Console.WriteLine("\tAdding " + termInfo);
                    var term = new Term();
                    var number = termInfo.Get("Number");
                    var ident = termInfo.Get("var");
                    if (number != null || ident != null)
                    {
                        if (number != null)
                        {
                            var token = number.AsChild.GetToken("value") as NumberToken;
                            term.value = token.Value;

                            if (number.AsChild.GetToken("signal") is SymbolToken signToken && signToken.Value == "-")
                                term.value *= -1;
                        }
                        else term.value = 1;

                        if (termInfo.GetToken("signal") is SymbolToken signT && signT.Value == "-")
                            term.value *= -1;

                        if (side == Term.Side.Right) term.value *= -1;

                        xTerms.Add(term);
                    }
                    else
                    {
                        var token = termInfo.GetToken("value") as NumberToken;
                        term.value = token.Value;

                        if (termInfo.GetToken("signal") is SymbolToken signToken && signToken.Value == "-")
                            term.value *= -1;

                        if (side == Term.Side.Left) term.value *= -1;

                        terms.Add(term);
                    }

                    Console.WriteLine("ADDED " + term.value + (number != null ? "x" : ""));
                    
                }

                if (pair.Value.AsChild != null) AddTermsOf(pair.Value.AsChild, side);
            }
        }
    }
}
