using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Examples.Equation
{
    public static class Main
    {
        public static void Run()
        {
            Console.WriteLine("Type in first degree equation to solve");
            string input = Console.ReadLine();
            if (Solver.TrySolve(input, out float result))
            {
                Console.WriteLine("x = " + result);
            }
            else
            {
                Console.WriteLine("Couldn't solve '" + input + "'");

                Console.WriteLine("Errors:");
                Console.WriteLine(Rules.Equation.LastErrors.ReduceToString("\n"));
            }
        }
    }
}
