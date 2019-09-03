using ParserFramework.Core;
using System;
using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
    public partial class Parser
    {
        public static Command Parse(string input)
        {
            var list = Rules.DefaultTokenList(input);
            var info = Rules.Command.Execute(list);
            Console.WriteLine(info);
            if (info == null)
            {
                Console.WriteLine(list);
                return null;
            }

            return CreateCommand(info);
        }
        public static Command CreateCommand(ParsingInfo info)
        {
            foreach (var pair in info)
            {
                if (pair.Key == "ExprAttr")
                {
                    return CreateAttribuition(info);
                }
                else if (pair.Key == "Print")
                {
                    return CreatePrintCommand(pair.Value.AsChild);
                }
                else Console.WriteLine("Command has '" + pair.Key + "'");
            }
            return null;
        }
        public static Attribuition CreateAttribuition(ParsingInfo info)
        {
            foreach (var pair in info)
            {
                if (pair.Key == "ExprAttr")
                {
                    return CreateExpressionAttribuition(pair.Value.AsChild);
                }
            }
            return null;
        }
        static ExpressionAttribuition CreateExpressionAttribuition(ParsingInfo info)
        {
            ExpressionAttribuition attr = new ExpressionAttribuition();
            foreach (var pair in info)
            {
                if (pair.Key == "varName")
                {
                    var token = pair.Value.AsToken as IdToken;
                    attr.varName = token.Value;
                }
                else if (pair.Key == "expr")
                {
                    attr.expression = CreateExpression(pair.Value.AsChild);
                }
            }
            return attr;
        }
        static PrintCommand CreatePrintCommand(ParsingInfo info)
        {
            PrintCommand cmd = new PrintCommand();
            foreach(var pair in info)
            {
                if(pair.Key == "arg")
                {
                    foreach (var child in pair.Value.AsChild)
                    {
                        foreach (var item in child.Value.AsChild)
                        {
                            if (item.Key == "expr")
                            {
                                cmd.ArgList.Add(CreateExpression(item.Value.AsChild));
                                break;
                            }
                            else if (item.Key == "string")
                            {
                                var token = item.Value.AsToken as Rules.StringToken;
                                cmd.ArgList.Add(token.Value);
                                break;
                            }
                            else Console.WriteLine(child.Key + " has '" + item.Key + "'");
                        }
                    }
                }
                else Console.WriteLine("Print has '" + pair.Key + "'");
            }
            return cmd;
        }
    }

    public abstract class Command { }
    public abstract class Attribuition : Command { }
    public class ExpressionAttribuition : Attribuition
    {
        public string varName;
        public Expression expression;

        public override string ToString()
        {
            return varName + " = " + expression.ToString();
        }
    }
    public class PrintCommand : Command
    {
        public class Arg
        {
            public Expression expression;
            public string str;
            public static implicit operator Arg(string str)
            {
                return new Arg() { str = str };
            }
            public static implicit operator Arg(Expression exp)
            {
                return new Arg() { expression = exp };
            }
        }
        public readonly List<Arg> ArgList = new List<Arg>();
        
        public override string ToString()
        {
            return "print " +
                ArgList.ReduceToString(arg => arg.str != null ? arg.str : arg.expression.ToString(), " ");
        }
    }
}
