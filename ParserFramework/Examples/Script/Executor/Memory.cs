using System.Collections.Generic;

namespace ParserFramework.Examples.Script
{
    public static class Memory
    {
        public static Dictionary<string, float> Variables { get; private set; } = new Dictionary<string, float>();

        public static void Save(string varName, float value)
        {
            if (Variables.ContainsKey(varName)) Variables.Remove(varName);
            Variables.Add(varName, value);
        }
        public static bool Get(string varName, out float value)
        {
            if (Variables.ContainsKey(varName))
            {
                value = Variables[varName];
                return true;
            }
            value = 0;
            return false;
        }
    }
}
