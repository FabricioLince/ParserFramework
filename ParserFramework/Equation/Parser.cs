using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Equation
{
    public class Parser
    {
        public static Equation Parse(string input)
        {
            var info = Rules.Main.Execute(Rules.DefaultTokenList(input));
            if (info == null) return null;
            //Console.WriteLine(info);
            return CreateEquation(info);
        }

        static Equation CreateEquation(ParsingInfo info)
        {
            if (info.FirstInfo.name == "Equation")
            {
                return CreateEquation(info.FirstInfo.AsChild);
            }
            Equation eq = new Equation();

            foreach (var pair in info)
            {
                if (pair.Key == "Add")
                {
                    eq.lhs = CreateAdd(pair.Value.AsChild);
                }
                else if (pair.Key == "Add_")
                {
                    eq.rhs = CreateAdd(pair.Value.AsChild);
                }
                else if (pair.Key == "equality") { }
                else
                { Console.WriteLine("Equation has '" + pair.Key + "'"); }
            }

            return eq;
        }

        static Add CreateAdd(ParsingInfo info)
        {
            Add add = new Add();
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
            foreach(var pair in info)
            {
                if(pair.Key == "op")
                {
                    var token = pair.Value.AsToken as SymbolToken;
                    op.operatorSymbol = token.Value;
                }
                else if(pair.Key=="Mult")
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
                if(pair.Key == "Term")
                {
                    mult.term = CreateTerm(pair.Value.AsChild);
                }
                else if(pair.Key == "mult_op")
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
                if(pair.Key == "op")
                {
                    var symbolToken = pair.Value.AsToken as SymbolToken;
                    op.operatorSymbol = symbolToken.Value;
                }
                else if(pair.Key=="Term")
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
                if(pair.Key == "Number")
                {
                    return CreateNumber(pair.Value.AsChild);
                }
                else if(pair.Key == "XTerm")
                {
                    return CreateXTerm(pair.Value.AsChild);
                }
                else if(pair.Key=="sub_expr")
                {
                    return CreateSubExpr(pair.Value.AsChild);
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
                    expr.add = CreateAdd(pair.Value.AsChild);
                }
                else if(pair.Key == "signal")
                {
                    var token = pair.Value.AsToken as SymbolToken;
                    expr.signal = token.Value;
                }
                else Console.WriteLine("SubExpr has '" + pair.Key + "'");
            }
            return expr;
        }

        static XTerm CreateXTerm(ParsingInfo info)
        {
            var xTerm = new XTerm() { value = 1 };
            var signal = "";
            foreach (var pair in info)
            {
                if (pair.Key == "Number")
                {
                    var number = CreateNumber(pair.Value.AsChild);
                    xTerm.value = number.value;
                }
                else if(pair.Key=="signal")
                {
                    var token = pair.Value.AsToken as SymbolToken;
                    signal = token.Value;
                }
                else if (pair.Key == "var") { }
                else Console.WriteLine("XTerm has '" + pair.Key + "'");
            }
            if (signal == "-") xTerm.value *= -1;
            return xTerm;
        }

        static Number CreateNumber(ParsingInfo info)
        {
            var number = new Number();
            var signal = "";
            foreach (var pair in info)
            {
                if(pair.Key == "value")
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
    }

    public class Equation
    {
        public Add lhs;
        public Add rhs;
        public override string ToString()
        {
            return lhs.ToString() + " = " + rhs.ToString();
        }
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
    public class XTerm : Term
    {
        public float value;
        public override string ToString()
        {
            return value + "x";
        }
    }
    public class SubExpr : Term
    {
        public string signal;
        public Add add;
        public override string ToString()
        {
            if (signal != null) return signal + "(" + add.ToString() + ")";

            return "(" + add.ToString() + ")";
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
    public class Add
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