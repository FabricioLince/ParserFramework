using ParserFramework.Core;
using System;
using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
    public partial class Parser
    {
        public static Command Parse(string input)
        {
            var list = Rules.DefaultTokenList(input);
            //Console.WriteLine(list);
            var info = Rules.Command.Execute(list);
            //Console.WriteLine(info);
            if (info == null)
            {
                Console.WriteLine(list);
                return null;
            }

            return CreateCommand(info);
        }
        public static Command CreateCommand(ParsingInfo info)
        {
            foreach (var pair in info)
            {
                if (pair.Key == "ExprAttr")
                {
                    return CreateAttribuition(info);
                }
                else if (pair.Key == "Print")
                {
                    return CreatePrintCommand(pair.Value.AsChild);
                }
                else if (pair.Key == "List")
                {
                    return CreateListCommand(pair.Value.AsChild);
                }
                else if (pair.Key == "IfCmd")
                {
                    return CreateIfCommand(pair.Value.AsChild);
                }
                else Console.WriteLine("Command has '" + pair.Key + "'");
            }
            return null;
        }
        public static Attribuition CreateAttribuition(ParsingInfo info)
        {
            foreach (var pair in info)
            {
                if (pair.Key == "ExprAttr")
                {
                    return CreateExpressionAttribuition(pair.Value.AsChild);
                }
            }
            return null;
        }
        static ExpressionAttribuition CreateExpressionAttribuition(ParsingInfo info)
        {
            ExpressionAttribuition attr = new ExpressionAttribuition();
            foreach (var pair in info)
            {
                if (pair.Key == "varName")
                {
                    var token = pair.Value.AsToken as IdToken;
                    attr.varName = token.Value;
                }
                else if (pair.Key == "expr")
                {
                    attr.expression = CreateExpression(pair.Value.AsChild);
                }
            }
            return attr;
        }
        static PrintCommand CreatePrintCommand(ParsingInfo info)
        {
            PrintCommand cmd = new PrintCommand();
            foreach(var pair in info)
            {
                if(pair.Key == "arg")
                {
                    foreach (var child in pair.Value.AsChild)
                    {
                        foreach (var item in child.Value.AsChild)
                        {
                            if (item.Key == "expr")
                            {
                                cmd.ArgList.Add(CreateExpression(item.Value.AsChild));
                                break;
                            }
                            else if (item.Key == "string")
                            {
                                var token = item.Value.AsToken as Rules.StringToken;
                                cmd.ArgList.Add(token.Value);
                                break;
                            }
                            else Console.WriteLine(child.Key + " has '" + item.Key + "'");
                        }
                    }
                }
                else Console.WriteLine("Print has '" + pair.Key + "'");
            }
            return cmd;
        }
        static ListCommand CreateListCommand(ParsingInfo info)
        {
            return new ListCommand();
        }
        static IfCommand CreateIfCommand(ParsingInfo info)
        {
            IfCommand cmd = new IfCommand();

            cmd.condition = CreateCondition(info.GetChild("condition"));
            if (cmd.condition == null)
            {
                PrintContentNames("IfCmd", info);
                throw new Exception("IfCmd doesn't have 'condition'");
            }

            cmd.command = CreateCommand(info.GetChild("Command"));
            if (cmd.command == null)
            {
                PrintContentNames("IfCmd", info);
                throw new Exception("IfCmd doesn't have 'command'");
            }

            cmd.elseCommand = CreateElse(info.GetChild("else"));

            return cmd;
        }
        static Command CreateElse(ParsingInfo info)
        {
            if (info == null) return null; // is optional
            return CreateCommand(info.GetChild("Command"));
        }

        static void PrintContentNames(string name, ParsingInfo info)
        {
            Console.WriteLine(name + " has " + GetContentNames(info));
        }
        static string GetContentNames(ParsingInfo info)
        {
            return info.ReduceToString(p => p.Key, ", ");
        }
    }

}
