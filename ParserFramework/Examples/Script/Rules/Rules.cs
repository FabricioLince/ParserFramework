using ParserFramework.Core;
using ParserFramework.Core.ParseRules;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserFramework.Examples.Script
{
    partial class Rules
    {
        public static ParseRule Command => new AlternateRule("Command")
        {
            possibilities = new List<ParseRule>()
            {
                CommandBlock,
                AttribuitionCommand,
                PrintCommand,
                ListCommand,
                IfCmd,
                ReadCmd,
                RunCmd,
                WhileCmd,
                BreakCmd,
                VarDeclCmd,
                FunDecl,
                FunCall,
            }
        };

        static ParseRule CommandBlock => new GroupRule("CmdBlock")
        {
            rulesF = new List<System.Func<ParseRule>>()
            {
                () => new SymbolRule("{"){ignore=true},
                () => new ChangeRuleKind(Command, ParseRule.Kind.Multiple, "Commands"),
                () => new SymbolRule("}"){ignore=true},
            }
        };

        static ParseRule AttribuitionCommand => new AlternateRule("Attr")
        {
            possibilities = new List<ParseRule>()
            {
                ExpressionAttribution,
            }
        };

        static ParseRule ExpressionAttribution => new GroupRule("ExprAttr")
        {
            rules = new List<ParseRule>()
            {
                new IdRule() { name = "varName" },
                new SymbolRule("=") { ignore = true },
                Expression("expr"),
            }
        };

        static ParseRule PrintCommand => new GroupRule("Print")
        {
            rules = new List<ParseRule>()
            {
                new IdRule("print"){ ignore = true },
                PrintArg,
                new SymbolRule(";"){ignore=true, kind = ParseRule.Kind.Optional}
            }
        };
        static ParseRule PrintArg => new AlternateRule("arg")
        {
            kind = ParseRule.Kind.Multiple,
            possibilities = new List<ParseRule>()
            {
                Expression("expr"),
                new TokenRule<StringToken>("string"),
            }
        };

        static ParseRule ListCommand => new GroupRule("List")
        {
            rules = new List<ParseRule>()
            {
                new IdRule("list")
            }
        };

        static ParseRule IfCmd => new GroupRule("IfCmd")
        {
            rulesF = new List<System.Func<ParseRule>>()
            {
                () => new IdRule("if"){ ignore=true },
                () => Condition,
                () => Command,
                () => ElseBlock
            }
        };
        static ParseRule ElseBlock => new GroupRule("else")
        {
            kind = ParseRule.Kind.Optional,
            rulesF = new List<System.Func<ParseRule>>()
            {
                () => new IdRule("else"){ignore=true},
                () => Command
            }
        };

        static ParseRule ReadCmd => new GroupRule("Read")
        {
            rules = new List<ParseRule>()
            {
                new IdRule("read"){ignore=true},
                new IdRule(){name = "varName"}
            }
        };

        static ParseRule RunCmd => new GroupRule("Run")
        {
            rules = new List<ParseRule>()
            {
                new IdRule("run"){ignore=true},
                new TokenRule<StringToken>(){name="fileName"}
            }
        };

        static ParseRule WhileCmd => new GroupRule("While")
        {
            rulesF = new List<System.Func<ParseRule>>()
            {
                () => new IdRule("while"){ignore=true},
                () => Condition,
                () => Command
            }
        };

        static ParseRule BreakCmd => new GroupRule("Break")
        {
            rules = new List<ParseRule>()
            {
                new IdRule("break")
            }
        };

        static ParseRule VarDeclCmd => new GroupRule("VarDecl")
        {
            rules = new List<ParseRule>()
            {
                Scope,
                new IdRule(){ name = "varName" },
                VarInit
            }
        };
        static ParseRule Scope => new AlternateRule("scope")
        {
            possibilities = new List<ParseRule>()
            {
                new IdRule("global", "local") { name = "scope" },
            }
        };
        static ParseRule VarInit => new GroupRule("VarInit")
        {
            kind = ParseRule.Kind.Optional,
            rules = new List<ParseRule>()
            {
                new SymbolRule("="),
                Expression("expr"),
            }
        };

        static ParseRule FunDecl => new GroupRule("FunDecl")
        {
            rulesF = new List<System.Func<ParseRule>>()
            {
                () => new IdRule("fun"),
                () => new IdRule(){name ="funName"},
                () => new SymbolRule("("),
                () => FunParam,
                () => new SymbolRule(")"),
                () => Command
            }
        };

        static ParseRule FunParam => new GroupRule("FunParam")
        {
            kind = ParseRule.Kind.Optional,
            rules = new List<ParseRule>()
            {
                new IdRule(){name = "param0"},
                new GroupRule("MoreParams")
                {
                    kind = ParseRule.Kind.Multiple,
                    rules = new List<ParseRule>()
                    {
                        new SymbolRule(","),
                        new IdRule(){name="param"}
                    }
                }
            }
        };

        static ParseRule FunCall => new GroupRule("FunCall")
        {
            rules = new List<ParseRule>()
            {
                new IdRule(){name="funName"},
                new SymbolRule("("),
                FunArgs,
                new SymbolRule(")"),
            }
        };
        static ParseRule FunArgs => new GroupRule("FunArgs")
        {
            kind = ParseRule.Kind.Optional,
            rules = new List<ParseRule>()
            {
                new NumberRule(){name = "arg0"},
                new GroupRule("MoreArgs")
                {
                    kind = ParseRule.Kind.Multiple,
                    rules = new List<ParseRule>()
                    {
                        new SymbolRule(","),
                        new NumberRule(){name="arg"}
                    }
                }
            }
        };

        public class StringToken : Token
        {
            public readonly string Value;
            public StringToken(string value) : base(Kind.CUSTOM)
            {
                this.Value = value;
            }
            public override string ToString()
            {
                return "STRING \"" + Value + "\"";
            }
        }

        public static TokenList DefaultTokenList(string input)
        {
            Tokenizer tokenizer = new Tokenizer(new StringReader(input));
            tokenizer.rules.Add(new Regex("^\'([^\"\n])*\'"), m => new StringToken(m.Value.Substring(1, m.Value.Length - 2)));
            tokenizer.rules.Add(new Regex("^\"([^\"\n])*\""), m => new StringToken(m.Value.Substring(1, m.Value.Length - 2)));

            tokenizer.rules.Add(new Regex(@"^([0-9]+)"), m => new IntToken(int.Parse(m.Value)));
            tokenizer.rules.Add(new Regex(@"^([0-9]+(?:\.[0-9]+)?)"), m => new FloatToken(float.Parse(m.Value.Replace('.', ','))));
            tokenizer.rules.Add(new Regex(@"^(\w+)"), m => new IdToken(m.Value));
            tokenizer.rules.Add(Core.Utils.RegexForSymbols("=="), m => new SymbolToken("=="));

            TokenList list = new TokenList(tokenizer);
            return list;
        }
    }
}
