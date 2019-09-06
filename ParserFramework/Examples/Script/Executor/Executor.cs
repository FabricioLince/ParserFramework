using System;
using System.Collections.Generic;
using System.IO;

namespace ParserFramework.Examples.Script
{
    partial class Executor
    {
        static bool breakSet = false;
        static int onLoop = 0;

        public static void Execute(string input)
        {
            var command = Parser.Parse(input);

            if (command != null)
            {
                Execute(command);
            }
            else
            {
                Console.WriteLine("Unrecognized command");
            }
        }

        public static void Execute(Command command)
        {
            if (command is PrintCommand p) ExecutePrint(p);
            else if (command is Attribuition attr) ExecuteAttribuition(attr);
            else if (command is ListCommand) ExecuteList();
            else if (command is IfCommand ifc) ExecuteIfCmd(ifc);
            else if (command is CommandBlock block) ExecuteBlock(block);
            else if (command is ReadCmd read) ExecuteRead(read);
            else if (command is RunCmd run) ExecuteRun(run);
            else if (command is WhileCmd w) ExecuteWhile(w);
            else if (command is BreakCmd) { if (onLoop > 0) breakSet = true; }
            else if (command is VarDeclCmd decl) ExecuteVarDecl(decl);
            else command.Execute();
        }

        private static void ExecuteWhile(WhileCmd w)
        {
            onLoop++;
            while (ConditionEvaluator.Evaluate(w.condition))
            {
                Execute(w.command);
                if(breakSet)
                {
                    breakSet = false;
                    break;
                }
            }
            onLoop--;
        }

        private static void ExecuteRun(RunCmd run)
        {
            try
            {
                string content = "{";
                using (StreamReader reader = new StreamReader(run.fileName))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        content += line + "\n";
                        line = reader.ReadLine();
                    }
                }
                content += "}";
                Execute(content);
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                //Console.WriteLine("File " + run.fileName + " couldn't be found");
            }
        }

        private static void ExecuteBlock(CommandBlock block)
        {
            Memory.IncreaseScope();
            foreach (var cmd in block.commands)
            {
                Execute(cmd);
                if (onLoop > 0 && breakSet) break;
            }
            Memory.DecreaseScope();
        }

        private static void ExecuteIfCmd(IfCommand ifc)
        {
            if (ConditionEvaluator.Evaluate(ifc.condition))
                Execute(ifc.command);
            else if (ifc.elseCommand != null)
                Execute(ifc.elseCommand);
        }

        static void ExecuteAttribuition(Attribuition attr)
        {
            if(attr is ExpressionAttribuition ea)
            {
                Memory.Save(ea.varName, ExpressionEvaluator.Evaluate(ea.expression));
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
            foreach (var v in Memory.Variables)
            {
                Console.WriteLine(v.Key + " = " + v.Value.value + " (" + v.Value.scope + ")");
            }
        }

        static void ExecuteRead(ReadCmd cmd)
        {
            string input = Console.ReadLine();
            if (float.TryParse(input, out float value))
            {
                Memory.Save(cmd.varName, value);
            }
        }

        static void ExecuteVarDecl(VarDeclCmd cmd)
        {
            if (Memory.Exists(cmd.varName))
            {
                Console.WriteLine(cmd.varName + " is already declared");
            }
            else
            {
                float value = 0;

                if (cmd.initializer != null)
                {
                    value = ExpressionEvaluator.Evaluate(cmd.initializer);
                }

                switch (cmd.scope)
                {
                    case VarDeclCmd.Scope.GLOBAL:
                        Memory.SaveGlobal(cmd.varName, value);
                        break;
                    case VarDeclCmd.Scope.LOCAL:
                        Memory.Save(cmd.varName, value);
                        break;
                }
            }
        }
    }
}
