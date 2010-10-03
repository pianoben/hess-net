using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace HessNet.Runtime.Serialization.Hessian
{
    public static class TypeExtensions
    {
        public static HessianType GetHessianType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsHessianString()) return HessianType.String;
            if (type.IsHessianBool())   return HessianType.Boolean;
            if (type.IsHessianInt())    return HessianType.Int;
            if (type.IsHessianLong())   return HessianType.Long;
            if (type.IsHessianList())   return HessianType.List;
            if (type.IsHessianDouble()) return HessianType.Double;
            if (type.IsHessianDate())   return HessianType.Date;
            if (type.IsHessianBinary()) return HessianType.Binary;
            if (type.IsHessianMap())    return HessianType.Map;
            if (type.IsHessianXml())    return HessianType.Xml;
            if (type.IsHessianList())   return HessianType.List;
            
            return HessianType.Object;
        }

        public static bool IsHessianBool(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return typeof(bool) == type;
        }

        public static bool IsHessianInt(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return typeof(int) == type
                || typeof(uint) == type
                || typeof(sbyte) == type
                || typeof(byte) == type
                || typeof(short) == type
                || typeof(ushort) == type;
        }

        public static bool IsHessianLong(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return typeof(long) == type
                || typeof(ulong) == type;
        }

        public static bool IsHessianDouble(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return typeof(float) == type
                || typeof(double) == type
                || typeof(decimal) == type;
        }

        public static bool IsHessianDate(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return typeof(DateTime) == type
                || typeof(DateTimeOffset) == type;
        }

        public static bool IsHessianBinary(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return (type.IsArray && typeof(byte) == type.GetElementType())
                || typeof(IList<byte>).IsAssignableFrom(type);
        }

        public static bool IsHessianString(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return typeof(string) == type
                || typeof(StringBuilder) == type;
        }

        public static bool IsHessianXml(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return typeof(XNode).IsAssignableFrom(type)
                || typeof(XmlNode).IsAssignableFrom(type);
        }

        public static bool IsHessianMap(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return typeof(IDictionary).IsAssignableFrom(type);
        }

        public static bool IsHessianList(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.IsArray
                || typeof(ICollection).IsAssignableFrom(type)
                || typeof(IList).IsAssignableFrom(type)
                || type.GetInterface("System.Collections.Generic.IList`1") != null;
        }
    }
}
