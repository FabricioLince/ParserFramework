using System;

namespace ParserFramework.Examples.Script
{
    public static class Main
    {
        public static void Run()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input == ".")
                {
                    MultiLineInput();
                }
                else if (input == "exit")
                {
                    break;
                }
                else
                {
                    Executor.Execute(input);
                }
            }
        }

        static void MultiLineInput()
        {
            string multiLineInput = "{\n"; // surround with {} for it to be treated as a command block
            while (true)
            {
                Console.Write(".");
                string input = Console.ReadLine();
                if (input == ".") break;
                multiLineInput += input + "\n";
            }
            multiLineInput += "}";
            Executor.Execute(multiLineInput);
        }

        
    }

    /*
     * 
     * cmd :: attr | print_cmd
     * attr :: expr_attr
     * expr_attr :: <id> = expr
     * print_cmd :: <id:print> print_arg+
     * print_arg :: expr | <string>
     * 
     * expr :: mult add_op*
     * add_op :: (+|-) mult
     * mult :: term mult_op*
     * mult_op :: (*|/) term
     * term :: number | variable | sub_expr
     * number :: [+|-] <number>
     * variable :: <id>
     * sub_expr :: [+|-] ( expr )
     * 
     * 
     * 
     * */
}
