using ParserFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Examples.Script
{
    public static class BaseParser
    {
        public static List<ParsingInfo> MultipleChildren(this ParsingInfo info, string childName)
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

        public static ParsingInfo MandatoryChild(this ParsingInfo info, string childName)
        {
            var child = info.GetChild(childName);
            AssertMandatory(info, child, childName);
            return child;
        }

        public static T MandatoryToken<T>(this ParsingInfo info, string tokenName) where T : Token
        {
            var token = info.GetToken(tokenName) as T;
            AssertMandatory(info, token, tokenName);
            return token;
        }

        public static void AssertMandatory(ParsingInfo info, object child, string childName)
        {
            if (child == null)
            {
                PrintContentNames(info);
                throw new Exception("Missing " + childName);
            }
        }

        public static void PrintContentNames(this ParsingInfo info)
        {
            Console.WriteLine(info.name + " has " + GetContentNames(info));
        }
        public static string GetContentNames(this ParsingInfo info)
        {
            return info.ReduceToString(p => p.Key + ":" + (p.Value.AsChild != null ? "child" : "token"), ", ");
        }
    }
}
