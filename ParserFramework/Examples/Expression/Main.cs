using System;

namespace ParserFramework.Examples.Expression
{
    public static class Main
    {
        public static void Run()
        {
            Console.WriteLine("Type in expression to evaluate");
            string input = Console.ReadLine();
            if (Solver.TrySolve(input, out float result))
            {
                Console.WriteLine("= " + result);
            }
            else
            {
                Console.WriteLine("Couldn't evaluate '" + input + "'");

                Console.WriteLine("Errors:");
                Console.WriteLine(Parser.Main.LastErrors.ReduceToString("\n"));
            }

        }
    }
}
