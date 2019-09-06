using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserFramework.Examples.Script
{
    public static partial class Memory
    {
        public class Variable
        {
            public int scope;
            float value;
            public float Value
            {
                get
                {
                    switch (type)
                    {
                        case Type.INT:
                            return (float)Math.Floor(value);
                        case Type.FLOAT:
                            return value;
                    }
                    return value;
                }
                set { this.value = value; }
            }
            public Type type = Type.INT;

            public const int GlobalScope = 0;

            public enum Type { INT, FLOAT };

            public Variable Copy()
            {
                return new Variable() { Value = this.Value, type = this.type };
            }

            public static Variable operator *(Variable a, Variable b)
            {
                return new Variable()
                {
                    Value = a.Value * b.Value,
                    type = a.type == Type.FLOAT ? Type.FLOAT : b.type
                };
            }
            public static Variable operator /(Variable a, Variable b)
            {
                return new Variable()
                {
                    Value = a.Value / b.Value,
                    type = a.type == Type.FLOAT ? Type.FLOAT : b.type
                };
            }
            public static Variable operator %(Variable a, Variable b)
            {
                return new Variable()
                {
                    Value = a.Value % b.Value,
                    type = Type.INT
                };
            }
            public static Variable operator +(Variable a, Variable b)
            {
                return new Variable()
                {
                    Value = a.Value + b.Value,
                    type = a.type == Type.FLOAT ? Type.FLOAT : b.type
                };
            }
            public static Variable operator -(Variable a, Variable b)
            {
                return new Variable()
                {
                    Value = a.Value - b.Value,
                    type = a.type == Type.FLOAT ? Type.FLOAT : b.type
                };
            }
        }
    }
}