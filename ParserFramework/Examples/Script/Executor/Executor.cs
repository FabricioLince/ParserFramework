using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Examples.Script
{
    partial class Executor
    {
        public static readonly Dictionary<string, float> variables = new Dictionary<string, float>();

        public static void Execute(Command command)
        {
            if (command is PrintCommand p) ExecutePrint(p);
            else if (command is Attribuition attr) ExecuteAttribuition(attr);
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
            foreach (var arg  in printCmd.ArgList)
            {
                if (arg.str != null)
                    Console.WriteLine(arg.str);
                else if (arg.expression != null)
                    Console.WriteLine(ExpressionEvaluator.Evaluate(arg.expression));
            }
            
        }
    }
}
