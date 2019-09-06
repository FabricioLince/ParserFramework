using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
    public static partial class Memory
    {
        public class Variable
        {
            public string name;
            public int scope;
            public float value;

            public const int GlobalScope = 0;
        }

        public static Dictionary<string, Variable> Variables { get; private set; } = new Dictionary<string, Variable>();

        public static int currentScope = 0;

        public static void Save(string varName, float value)
        {
            if (Variables.ContainsKey(varName))// Check scope??
            {
                Variables[varName].value = value;
                return;
            }
            Variables.Add(varName, 
                new Variable()
                {
                    name = varName,
                    value = value,
                    scope = currentScope
                });
        }
        public static void SaveGlobal(string varName, float value)
        {
            if (Variables.ContainsKey(varName)) // Check scope??
            {
                Variables[varName].value = value;
                return;
            }
            Variables.Add(varName,
                new Variable()
                {
                    name = varName,
                    value = value,
                    scope = 0
                });
        }
        public static bool Get(string varName, out float value)
        {
            if (Variables.ContainsKey(varName))
            {
                value = Variables[varName].value;
                return true;
            }
            value = 0;
            return false;
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
