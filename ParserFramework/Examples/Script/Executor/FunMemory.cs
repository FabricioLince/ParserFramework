using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Examples.Script
{
    public static partial class Memory
    {
        public class Function
        {
            public string name;
            public List<string> parameters = new List<string>();
            public Command command;
        }

        public static Dictionary<string, Function> Functions { get; private set; } = new Dictionary<string, Function>();

        public static void Call(string funName, params float[] args)
        {
            if (!Functions.ContainsKey(funName)) return;
            var fun = Functions[funName];

            if (args.Length != fun.parameters.Count)
            {

            }
            Memory.IncreaseScope();

            for (int i = 0; i < args.Length; ++i)
            {
                Memory.Save(fun.parameters[i], args[i]);
            }

            Executor.Execute(fun.command);

            Memory.DecreaseScope();
        }
    }
}
