using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
    public abstract class Command { public virtual void Execute() { } }
    public class DoNothingCmd : Command { }
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

    public class CommandBlock : Command
    {
        public List<Command> commands = new List<Command>();
    }

    public class ReadCmd : Command
    {
        public string varName;
    }

    public class RunCmd : Command
    {
        public string fileName;
    }

    public class WhileCmd : Command
    {
        public Condition condition;
        public Command command;
    }

    public class BreakCmd : Command
    {

    }

    public class VarDeclCmd : Command
    {
        public enum Scope { GLOBAL, LOCAL }
        public Scope scope = Scope.LOCAL;
        public string varName;
        public Expression initializer;
    }

    public class FunDeclCmd : Command
    {
        public string funName;
        public List<string> parameters = new List<string>();
        public Command command;

        public override void Execute()
        {
            if(Memory.Functions.ContainsKey(funName))
            {
                System.Console.WriteLine("Overwriting fun " + funName);
                Memory.Functions.Remove(funName);
            }
            Memory.Functions.Add(funName, new Memory.Function()
            {
                name = funName,
                parameters = parameters,
                command = command
            });
        }

        public override string ToString()
        {
            return "fun " + funName + "(" + parameters.ReduceToString(", ") + ")";
        }
    }

    public class FunCallCmd:Command
    {
        public string funName;
        public List<float> args = new List<float>();

        public override void Execute()
        {
            Memory.Call(funName, args.ToArray());
        }

        public override string ToString()
        {
            return "Call " + funName + "(" + args.ReduceToString(v => v.ToString(), ", ") + ")";
        }
    }
}