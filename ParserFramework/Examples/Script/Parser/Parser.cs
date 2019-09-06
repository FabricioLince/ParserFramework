using ParserFramework.Core;
using System;
using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
    public static partial class Parser
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
                else if (pair.Key == "CmdBlock")
                {
                    return CreateCommandBlock(pair.Value.AsChild);
                }
                else if (pair.Key == "Read")
                {
                    return CreateReadCommand(pair.Value.AsChild);
                }
                else if (pair.Key == "Run")
                {
                    return CreateRunCommand(pair.Value.AsChild);
                }
                else if (pair.Key == "While")
                {
                    return CreateWhileCommand(pair.Value.AsChild);
                }
                else if (pair.Key == "Break")
                {
                    return new BreakCmd();
                }
                else if (pair.Key == "VarDecl")
                {
                    return CreateVarDeclCommand(pair.Value.AsChild);
                }
                else if (pair.Key == "FunDecl")
                {
                    return CreateFunDecl(pair.Value.AsChild);
                }
                else if (pair.Key == "FunCall")
                {
                    return CreateFunCall(pair.Value.AsChild);
                }
                else Console.WriteLine("Command has '" + pair.Key + "'");
            }
            return new DoNothingCmd();
        }

        private static Command CreateWhileCommand(ParsingInfo info)
        {
            var cmd = new WhileCmd();

            cmd.condition = CreateCondition(info.MandatoryChild("condition"));

            cmd.command = CreateCommand(info.MandatoryChild("Command"));

            return cmd;
        }

        private static Command CreateRunCommand(ParsingInfo info)
        {
            var cmd = new RunCmd();

            cmd.fileName = info.MandatoryToken<Rules.StringToken>("fileName").Value;

            return cmd;
        }

        public static Attribuition CreateAttribuition(ParsingInfo info)
        {
            var exprAttr = info.GetChild("ExprAttr");
            if (exprAttr != null)
            {
                return CreateExpressionAttribuition(exprAttr);
            }
            return null;
        }
        static ExpressionAttribuition CreateExpressionAttribuition(ParsingInfo info)
        {
            ExpressionAttribuition attr = new ExpressionAttribuition();

            attr.varName = info.MandatoryToken<IdToken>("varName").Value;

            attr.expression = CreateExpression(info.MandatoryChild("expr"));

            return attr;
        }
        static PrintCommand.Arg CreateArg(ParsingInfo info)
        {
            var expr = info.GetChild("expr");
            if (expr != null)
            {
                return new PrintCommand.Arg() { expression = CreateExpression(expr) };
            }
            var str = info.GetToken("string") as Rules.StringToken;
            if (str != null)
            {
                return new PrintCommand.Arg() { str = str.Value };
            }

            info.PrintContentNames();
            throw new Exception("Unrecognized print arg");
        }
        static PrintCommand CreatePrintCommand(ParsingInfo info)
        {
            PrintCommand cmd = new PrintCommand();

            var args = info.MultipleChildren("arg");
            foreach (var arg in args)
            {
                cmd.ArgList.Add(CreateArg(arg));
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

            cmd.condition = CreateCondition(info.MandatoryChild("condition"));

            cmd.command = CreateCommand(info.MandatoryChild("Command"));

            cmd.elseCommand = CreateElse(info.GetChild("else"));

            return cmd;
        }
        static Command CreateElse(ParsingInfo info)
        {
            if (info == null) return null; // is optional
            return CreateCommand(info.GetChild("Command"));
        }

        static Command CreateCommandBlock(ParsingInfo info)
        {
            CommandBlock block = new CommandBlock();

            var commands = info.MultipleChildren("Commands");
            foreach (var cmd in commands)
            {
                block.commands.Add(CreateCommand(cmd));
                //Console.WriteLine("created command " + block.commands[block.commands.Count - 1]);
            }
            return block;
        }

        static Command CreateReadCommand(ParsingInfo info)
        {
            ReadCmd cmd = new ReadCmd();
            cmd.varName = info.MandatoryToken<IdToken>("varName").Value;

            return cmd;
        }

        static Command CreateVarDeclCommand(ParsingInfo info)
        {
            VarDeclCmd cmd = new VarDeclCmd();

            var scopeToken = info.MandatoryToken<IdToken>("scope");
            switch (scopeToken.Value)
            {
                case "local":
                    cmd.scope = VarDeclCmd.Scope.LOCAL;
                    break;
                case "global":
                    cmd.scope = VarDeclCmd.Scope.GLOBAL;
                    break;
            }

            var varNameToken = info.MandatoryToken<IdToken>("varName");
            cmd.varName = varNameToken.Value;

            var init = info.GetChild("VarInit");
            if (init != null)
            {
                var expr = init.MandatoryChild("expr");
                cmd.initializer = CreateExpression(expr);
            }

            return cmd;
        }

        static Command CreateFunDecl(ParsingInfo info)
        {
            var funDecl = new FunDeclCmd();

            var nameToken = info.MandatoryToken<IdToken>("funName");
            funDecl.funName = nameToken.Value;

            var parameters = info.GetChild("FunParam");
            if (parameters != null)
            {
                var argToken = parameters.MandatoryToken<IdToken>("param0");
                funDecl.parameters.Add(argToken.Value);
                var otherArgs = parameters.MultipleChildren("MoreParams");
                foreach (var otherArg in otherArgs)
                {
                    funDecl.parameters.Add(otherArg.MandatoryToken<IdToken>("param").Value);
                }
            }

            funDecl.command = CreateCommand(info.MandatoryChild("Command"));

            Console.WriteLine(funDecl);

            return funDecl;
        }

        static Command CreateFunCall(ParsingInfo info)
        {
            var funCall = new FunCallCmd();

            funCall.funName = info.MandatoryToken<IdToken>("funName").Value;

            var args = info.GetChild("FunArgs");
            if (args != null)
            {
                funCall.args.Add(CreateExpression(args.MandatoryChild("arg0")));

                var otherArgs = args.MultipleChildren("MoreArgs");
                foreach (var otherArg in otherArgs)
                {
                    funCall.args.Add(CreateExpression(args.MandatoryChild("arg")));
                }
            }

            Console.WriteLine(funCall);

            return funCall;
        }
    }

}