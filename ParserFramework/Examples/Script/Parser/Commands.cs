using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
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
    public class ListCommand : Command
    {
        public override string ToString()
        {
            return "list";
        }
    }

    public class IfCommand : Command
    {
        public Condition condition;
        public Command command;
        public Command elseCommand;

        public override string ToString()
        {
            return "if " + condition.ToString() + "\n" + command.ToString() + "\nelse\n" + elseCommand.ToString();
        }
    }
}
