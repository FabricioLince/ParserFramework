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
        public static Command Parse(string input)
        {
            var list = Rules.DefaultTokenList(input);
            var info = Rules.Command.Execute(list);
            Console.WriteLine(info);
            if (info == null) return null;
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
                else if (pair.Key=="Print")
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
                if(pair.Key == "expr")
                {
                    cmd.expression = CreateExpression(pair.Value.AsChild);
                }
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
        public Expression expression;
        public override string ToString()
        {
            return "print " + expression.ToString();
        }
    }
}
