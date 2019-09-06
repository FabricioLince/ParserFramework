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
                else if(pair.Key == "FunCall")
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

            var condition = info.GetChild("condition");
            if (condition == null)
            {
                PrintContentNames("WhileCmd", info);
                throw new Exception("condition missing");
            }
            cmd.condition = CreateCondition(condition);

            var command = info.GetChild("Command");
            if (command == null)
            {
                PrintContentNames("WhileCmd", info);
                throw new Exception("command missing");
            }
            cmd.command = CreateCommand(command);

            return cmd;
        }

        private static Command CreateRunCommand(ParsingInfo info)
        {
            var cmd = new RunCmd();

            var stringToken = info.GetToken("fileName") as Rules.StringToken;
            if (stringToken == null)
            {
                PrintContentNames("RunCmd", info);
                throw new Exception("fileName missing");
            }

            cmd.fileName = stringToken.Value;

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

            var varToken = info.GetToken("varName") as IdToken;
            if (varToken == null)
            {
                throw new Exception("no varName");
            }
            attr.varName = varToken.Value;

            attr.expression = CreateExpression(info.GetChild("expr"));
            if (attr.expression == null)
            {
                throw new Exception("no expr");
            }

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

            PrintContentNames("arg", info);
            throw new Exception("Unrecognized print arg");
        }
        static PrintCommand CreatePrintCommand(ParsingInfo info)
        {
            PrintCommand cmd = new PrintCommand();

            ParsingInfo args = info.GetChild("arg");
            if (args != null) // multiple
            {
                foreach (var arg in args)
                {
                    cmd.ArgList.Add(CreateArg(arg.Value.AsChild));
                }
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
                throw new Exception("IfCmd doesn't have 'Command'");
            }

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
            var commands = info.GetChild("Commands");
            if (commands != null) // Multiple
            {
                foreach (var cmd in commands)
                {
                    block.commands.Add(CreateCommand(cmd.Value.AsChild));
                }
            }
            return block;
        }

        static Command CreateReadCommand(ParsingInfo info)
        {
            ReadCmd cmd = new ReadCmd();
            var idToken = info.GetToken("varName") as IdToken;
            if (idToken == null)
            {
                PrintContentNames("read", info);
                throw new Exception("varName missing");
            }
            cmd.varName = idToken.Value;

            return cmd;
        }

        static Command CreateVarDeclCommand(ParsingInfo info)
        {
            VarDeclCmd cmd = new VarDeclCmd();
            
            var scopeToken = info.MandatoryToken<IdToken>("VarDecl", "scope");
            switch(scopeToken.Value)
            {
                case "local":
                    cmd.scope = VarDeclCmd.Scope.LOCAL;
                    break;
                case "global":
                    cmd.scope = VarDeclCmd.Scope.GLOBAL;
                    break;
            }

            var varNameToken = info.MandatoryToken<IdToken>("VarDecl", "varName");
            cmd.varName = varNameToken.Value;

            var init = info.GetChild("VarInit");
            if(init!=null)
            {
                var expr = init.MandatoryChild("VarDecl", "expr");
                cmd.initializer = CreateExpression(expr);
            }

            return cmd;
        }

        static Command CreateFunDecl(ParsingInfo info)
        {
            var funDecl = new FunDeclCmd();

            var nameToken = info.MandatoryToken<IdToken>("FunDecl", "funName");
            funDecl.funName = nameToken.Value;

            var parameters = info.GetChild("FunParam");
            if (parameters != null)
            {
                var argToken = parameters.MandatoryToken<IdToken>("FunParam", "param0");
                funDecl.parameters.Add(argToken.Value);
                var otherArgs = parameters.MultipleChildren("FunParam", "MoreParams");
                foreach (var otherArg in otherArgs)
                {
                    funDecl.parameters.Add(otherArg.MandatoryToken<IdToken>("FunParam", "param").Value);
                }
            }

            funDecl.command = CreateCommand(info.MandatoryChild("FunDecl", "Command"));

            //Console.WriteLine(funDecl);

            return funDecl;
        }

        static Command CreateFunCall(ParsingInfo info)
        {
            var funCall = new FunCallCmd();

            funCall.funName = info.MandatoryToken<IdToken>("FunCall", "funName").Value;

            var args = info.GetChild("FunArgs");
            if (args != null)
            {
                funCall.args.Add(args.MandatoryToken<NumberToken>("FunArgs", "arg0").Value);

                var otherArgs = args.MultipleChildren("FunArgs", "MoreArgs");
                foreach(var otherArg in otherArgs)
                {
                    funCall.args.Add(otherArg.MandatoryToken<NumberToken>("FunArgs", "arg").Value);
                }
            }

            //Console.WriteLine(funCall);

            return funCall;
        }

        static List<ParsingInfo> MultipleChildren(this ParsingInfo info, string infoName, string childName)
        {
            var list = new List<ParsingInfo>();
            var chldren = info.GetChild(childName);
            if (chldren != null) // Multiple
            {
                foreach (var child in chldren)
                {
                    list.Add(child.Value.AsChild);
                }
            }
            return list;
        }

        static ParsingInfo MandatoryChild(this ParsingInfo info, string infoName, string childName)
        {
            var child = info.GetChild(childName);
            AssertMandatory(info, infoName, child, childName);
            return child;
        }

        static T MandatoryToken<T>(this ParsingInfo info, string infoName, string tokenName) where T : Token
        {
            var token = info.GetToken(tokenName) as T;
            AssertMandatory(info, infoName, token, tokenName);
            return token;
        }

        static void AssertMandatory(ParsingInfo info, string infoName, object child, string childName)
        {
            if (child == null)
            {
                PrintContentNames(infoName, info);
                throw new Exception("Missing " + childName);
            }
        }

        static void PrintContentNames(string name, ParsingInfo info)
        {
            Console.WriteLine(name + " has " + GetContentNames(info));
        }
        static string GetContentNames(ParsingInfo info)
        {
            return info.ReduceToString(p => p.Key + ":" + (p.Value.AsChild != null ? "child" : "token"), ", ");
        }
    }

}
