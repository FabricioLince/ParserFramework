using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
    public static partial class Memory
    {
        public static Dictionary<string, Variable> Variables { get; private set; } = new Dictionary<string, Variable>();

        public static int currentScope = 0;

        public static void Save(string varName, Variable v)
        {
            if (Variables.ContainsKey(varName))// Check scope an type??
            {
                int scope = Variables[varName].scope;
                Variables[varName] = v;
                Variables[varName].scope = scope;
                return;
            }
            Variables.Add(varName,
                new Variable()
                {
                    //name = varName,
                    Value = v.Value,
                    type = v.type,
                    scope = currentScope
                });
            //System.Console.WriteLine("Saving " + varName + " scope=" + currentScope);
        }
        public static void Save(string varName, float value)
        {
            Save(varName, new Variable() { /*name = varName,*/ type = Variable.Type.FLOAT, Value = value });
        }
        public static void SaveGlobal(string varName, float value)
        {
            Save(varName, value);
            Variables[varName].scope = Variable.GlobalScope;
        }
        public static void SaveGlobal(string varName, Variable v)
        {
            Save(varName, v);
            Variables[varName].scope = Variable.GlobalScope;
        }
        public static bool Get(string varName, out float value)
        {
            if (Variables.ContainsKey(varName))
            {
                value = Variables[varName].Value;
                return true;
            }
            value = 0;
            return false;
        }
        public static Variable Get(string varName)
        {
            if (Exists(varName)) return Variables[varName];
            return null;
        }
        public static bool Exists(string varName) => Variables.ContainsKey(varName);

        public static void IncreaseScope()
        {
            currentScope++;
        }
        public static void DecreaseScope()
        {
            if (currentScope == Variable.GlobalScope) return;

            // remove all variable of current scope
            var list = new List<string>();
            foreach (var pair in Variables) if (pair.Value.scope == currentScope) list.Add(pair.Key);
            foreach(var name in list) Variables.Remove(name);

            currentScope--;
        }

    }
}
