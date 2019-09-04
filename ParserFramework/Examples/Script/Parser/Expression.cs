using ParserFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Examples.Script
{
    public partial class Parser
    {
        static Expression CreateExpression(ParsingInfo info)
        {
            Expression add = new Expression();
            foreach (var pair in info)
            {
                if (pair.Key == "Mult")
                {
                    add.mult = CreateMult(pair.Value.AsChild);
                }
                else if (pair.Key == "add_op")
                {
                    foreach (var child in pair.Value.AsChild)
                    {
                        add.addOp.Add(CreateAddOp(child.Value.AsChild));
                    }
                }
                else Console.WriteLine("Add has '" + pair.Key + "'");
            }
            return add;
        }

        static AddOp CreateAddOp(ParsingInfo info)
        {
            AddOp op = new AddOp();
            foreach (var pair in info)
            {
                if (pair.Key == "op")
                {
                    var token = pair.Value.AsToken as SymbolToken;
                    op.operatorSymbol = token.Value;
                }
                else if (pair.Key == "Mult")
                {
                    op.mult = CreateMult(pair.Value.AsChild);
                }
                else Console.WriteLine("AddOp has '" + pair.Key + "'");
            }
            return op;
        }

        static Mult CreateMult(ParsingInfo info)
        {
            Mult mult = new Mult();
            foreach (var pair in info)
            {
                if (pair.Key == "Term")
                {
                    mult.term = CreateTerm(pair.Value.AsChild);
                }
                else if (pair.Key == "mult_op")
                {
                    foreach (var child in pair.Value.AsChild)
                    {
                        mult.multOp.Add(CreateMultOp(child.Value.AsChild));
                    }
                }
                else Console.WriteLine("Mult has '" + pair.Key + "'");
            }
            return mult;
        }

        static MultOp CreateMultOp(ParsingInfo info)
        {
            MultOp op = new MultOp();
            foreach (var pair in info)
            {
                if (pair.Key == "op")
                {
                    var symbolToken = pair.Value.AsToken as SymbolToken;
                    op.operatorSymbol = symbolToken.Value;
                }
                else if (pair.Key == "Term")
                {
                    op.term = CreateTerm(pair.Value.AsChild);
                }
                else Console.WriteLine("Mult has '" + pair.Key + "'");
            }
            return op;
        }

        static Term CreateTerm(ParsingInfo info)
        {
            Term term = new Number() { value = 1337 };
            foreach (var pair in info)
            {
                if (pair.Key == "Number")
                {
                    return CreateNumber(pair.Value.AsChild);
                }
                else if (pair.Key == "sub_expr")
                {
                    return CreateSubExpr(pair.Value.AsChild);
                }
                else if (pair.Key == "Variable")
                {
                    return CreateVar(pair.Value.AsChild);
                }
                else Console.WriteLine("Term has '" + pair.Key + "'");
            }
            return term;
        }

        static SubExpr CreateSubExpr(ParsingInfo info)
        {
            SubExpr expr = new SubExpr();
            foreach (var pair in info)
            {
                if (pair.Key == "Add")
                {
                    expr.add = CreateExpression(pair.Value.AsChild);
                }
                else if (pair.Key == "signal")
                {
                    var token = pair.Value.AsToken as SymbolToken;
                    expr.signal = token.Value;
                }
                else Console.WriteLine("SubExpr has '" + pair.Key + "'");
            }
            return expr;
        }

        static Number CreateNumber(ParsingInfo info)
        {
            var number = new Number();
            var signal = "";
            foreach (var pair in info)
            {
                if (pair.Key == "value")
                {
                    var valueToken = pair.Value.AsToken as NumberToken;
                    number.value = valueToken.Value;
                }
                else if (pair.Key == "signal")
                {
                    var token = pair.Value.AsToken as SymbolToken;
                    signal = token.Value;
                }
                else Console.WriteLine("Number has '" + pair.Key + "'");
            }
            if (signal == "-") number.value *= -1;
            return number;
        }

        static Variable CreateVar(ParsingInfo info)
        {
            var variable = new Variable();

            foreach (var pair in info)
            {
                if (pair.Key == "varName")
                {
                    var token = pair.Value.AsToken as IdToken;
                    variable.varName = token.Value;
                }
                else Console.WriteLine("Var has '" + pair.Key + "'");
            }

            return variable;
        }

        public static Condition CreateCondition(ParsingInfo info)
        {
            var condition = new Condition();
            condition.expr = CreateExpression(info.GetChild("expr"));
            if (condition.expr == null)
            {
                Console.WriteLine(info.ReduceToString(p => "condition has '" + p.Key + "'", "\n"));
                throw new Exception("Condition doesn't have 'expr'");
            }

            condition.comparation = CreateComparation(info.GetChild("comparation"));

            return condition;
        }
        static Comparation CreateComparation(ParsingInfo info)
        {
            if (info == null) return null; // Is Optional

            var comparation = new Comparation();
            if (!(info.GetToken("signal") is SymbolToken signalToken)) throw new Exception("Comparation w/o signal");
            comparation.signal = signalToken.Value;
            comparation.expr = CreateExpression(info.GetChild("expr"));
            if (comparation.expr == null)
            {
                Console.WriteLine(info.ReduceToString(p => "condition has '" + p.Key + "'", "\n"));
                throw new Exception("Condition doesn't have 'expr'");
            }

            return comparation;
        }
    }

    public class Condition
    {
        public Expression expr;
        public Comparation comparation;
    }
    public class Comparation
    {
        public string signal;
        public Expression expr;
    }

    public abstract class Term { }
    public class Number : Term
    {
        public float value;
        public override string ToString()
        {
            return value.ToString();
        }
    }
    public class SubExpr : Term
    {
        public string signal;
        public Expression add;
        public override string ToString()
        {
            if (signal != null) return signal + "(" + add.ToString() + ")";

            return "(" + add.ToString() + ")";
        }
    }
    public class Variable : Term
    {
        public string varName;
        public override string ToString()
        {
            return varName;
        }
    }
    public class Mult
    {
        public Term term;
        public List<MultOp> multOp = new List<MultOp>();
        public override string ToString()
        {
            string rt = term.ToString();
            foreach (var op in multOp)
            {
                rt += op.ToString();
            }
            return rt;
        }
    }
    public class MultOp
    {
        public string operatorSymbol;
        public Term term;
        public override string ToString()
        {
            return operatorSymbol + term.ToString();
        }
    }
    public class Expression
    {
        public Mult mult;
        public List<AddOp> addOp = new List<AddOp>();
        public override string ToString()
        {
            string rt = mult.ToString();
            foreach (var op in addOp)
            {
                rt += op.ToString();
            }
            return rt;
        }
    }
    public class AddOp
    {
        public string operatorSymbol;
        public Mult mult;
        public override string ToString()
        {
            return operatorSymbol + "" + mult.ToString();
        }
    }
}