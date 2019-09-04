using System;
using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
    partial class Executor
    {
        public static readonly Dictionary<string, float> variables = new Dictionary<string, float>();

        public static void Execute(Command command)
        {
            if (command is PrintCommand p) ExecutePrint(p);
            else if (command is Attribuition attr) ExecuteAttribuition(attr);
            else if (command is ListCommand) ExecuteList();
            else if (command is IfCommand ifc) ExecuteIfCmd(ifc);
        }

        private static void ExecuteIfCmd(IfCommand ifc)
        {
            float lhs = ExpressionEvaluator.Evaluate(ifc.condition.expr);
            if (ifc.condition.comparation == null)
            {
                // no rhs on condition, evaluate lhs as bool
                Execute(lhs != 0, ifc);
            }
            else
            {
                float rhs = ExpressionEvaluator.Evaluate(ifc.condition.comparation.expr);
                switch(ifc.condition.comparation.signal)
                {
                    case ">":
                        Execute(lhs > rhs, ifc);
                        break;
                    case "<":
                        Execute(lhs < rhs, ifc);
                        break;
                    case "==":
                        Execute(lhs == rhs, ifc);
                        break;
                }
            }
        }
        static void Execute(bool condition, IfCommand ifc)
        {
            if (condition)
                Execute(ifc.command);
            else if (ifc.elseCommand != null)
                Execute(ifc.elseCommand);
        }

        static void ExecuteAttribuition(Attribuition attr)
        {
            if(attr is ExpressionAttribuition ea)
            {
                if (variables.ContainsKey(ea.varName)) variables.Remove(ea.varName);
                variables.Add(ea.varName, ExpressionEvaluator.Evaluate(ea.expression));
            }
        }

        static void ExecutePrint(PrintCommand printCmd)
        {
            Console.WriteLine(printCmd.ArgList.ReduceToString(arg =>
            {
                if (arg.str != null)
                    return arg.str;
                else if (arg.expression != null)
                    return ExpressionEvaluator.Evaluate(arg.expression).ToString();
                return "";
            }, " "));
        }

        static void ExecuteList()
        {
            foreach (var v in Executor.variables)
            {
                Console.WriteLine(v.Key + " = " + v.Value);
            }
        }
    }
}
