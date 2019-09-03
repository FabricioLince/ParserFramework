using System;

namespace ParserFramework.Examples.Script
{
    public static class Main
    {
        public static void Run()
        {
            Executor.Execute(Parser.Parse("a=2*2"));
            Executor.Execute(Parser.Parse("print 12"));

            while (true)
            {
                string input = Console.ReadLine();

                var command = Parser.Parse(input);

                if (command != null)
                {
                    Executor.Execute(command);
                }
                else if (input == "exit")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Unrecognized command");
                }
            }
        }
    }
}
