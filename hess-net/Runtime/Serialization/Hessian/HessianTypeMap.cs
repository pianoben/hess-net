using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class HessianTypeMap
    {
        public HessianTypeMap(Type type)
        {
            var dataContract = type.GetCustomAttributes(typeof(DataContractAttribute), false);

            if (dataContract.Length == 0)
            {

            }
        }

        private class Member
        {
            private readonly string contractName;
            private readonly MemberInfo mi;
            private readonly bool isProperty;
            private readonly Type memberType;
            private readonly List<Member> members;

            public string ContractName
            {
                get { return contractName; }
            }

            public Type MemberType
            {
                get { return memberType; }
            }

            public Member(string contractName, MemberInfo mi, Type memberType)
            {
                isProperty = mi is PropertyInfo;
                members = new List<Member>();
            }

            public object GetValueFor(object o)
            {
                if (isProperty)
                {
                    return ((PropertyInfo)mi).GetValue(o, null);
                }
                else
                {
                    return ((FieldInfo)mi).GetValue(o);
                }
            }

            public void SetValueFor(object o, object value)
            {
                if (isProperty)
                {
                    ((PropertyInfo)mi).SetValue(o, value, null);
                }
                else
                {
                    ((FieldInfo)mi).SetValue(o, value);
                }
            }
        }
    }
}
