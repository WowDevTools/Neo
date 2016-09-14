using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DBCLib
{
    public class DBCHelper
    {
        public static int FieldCount(FieldInfo[] fields, Type type)
        {
            var t = Activator.CreateInstance(type);
            var fieldCount = 0;
            foreach (var field in fields)
            {
                if (Type.GetTypeCode(field.FieldType) == TypeCode.Object)
                {
                    if (field.FieldType == typeof(LocalizedString))
                        fieldCount += 17;
                    else if (field.FieldType.IsArray)
                        switch (Type.GetTypeCode(field.FieldType.GetElementType()))
                        {
                            case TypeCode.Int32:
                                fieldCount += ((int[])field.GetValue(t)).Length;
                                break;
                            case TypeCode.UInt32:
                                fieldCount += ((uint[])field.GetValue(t)).Length;
                                break;
                            case TypeCode.Single:
                                fieldCount += ((float[])field.GetValue(t)).Length;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                }
                else
                    ++fieldCount;
            }
            return fieldCount;
        }
    }

    public class LocalizedString
    {
        public string String;

        public LocalizedString(String s)
        {
            String = s;
        }

        public static implicit operator LocalizedString(string s)
        {
            return new LocalizedString(s);
        }

        public static implicit operator String(LocalizedString s)
        {
            return s.String;
        }
    }
}
