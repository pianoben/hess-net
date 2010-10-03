using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class SerializationPlan
    {
        #region Factory implementation

        private static Dictionary<Type, SerializationPlan> __plans = new Dictionary<Type, SerializationPlan>();

        static SerializationPlan()
        {
            
        }

        public static SerializationPlan GetForType(Type toSerialize)
        {
            if (toSerialize == null)
                throw new ArgumentNullException("toSerialize");

            return __plans.ContainsKey(toSerialize)
                ? __plans[toSerialize]
                : __plans[toSerialize] = new SerializationPlan(toSerialize);
        }

        #endregion Factory implementation

        private readonly Type serializes;
        private readonly string contractName;
        private readonly Collection<SerializationMember> members;

        /// <summary>
        /// Gets the <see cref="Type"/> (de)serialized by this
        /// <see cref="SerializationPlan"/>.
        /// </summary>
        public Type PlanType
        {
            get { return serializes; }
        }

        public string TypeName
        {
            get { return contractName; }
        }

        public void WriteObject(HessianWriter serializationWriter, object target)
        {
            foreach (var member in members)
            {

            }
        }

        public object ReadObject(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        protected SerializationPlan(Type t)
        {
            var attributes = t.GetCustomAttributes(typeof(DataContractAttribute), false);

            if (attributes.Length != 1)
                throw new ArgumentException("The given type does not have a DataContractAttribute.");

            var contract = (DataContractAttribute)attributes[0];

            this.contractName = GetContractName(contract, t);
            this.members = new Collection<SerializationMember>();

            foreach (var field in from f in t.GetFields().Cast<MemberInfo>().Concat(t.GetProperties())
                                  let attrs = f.GetCustomAttributes(typeof(DataMemberAttribute), false)
                                  where attrs.Length == 1
                                  let member = (DataMemberAttribute)attrs[0]
                                  orderby member.Order ascending
                                  select new { Info = f, Contract = member })
            {
                SerializationMember member;
                var name = GetMemberName(field.Info, field.Contract);

                switch (field.Info.MemberType)
                {
                    case MemberTypes.Field:
                        member = new FieldMember(name, (FieldInfo)field.Info);
                        break;
                    case MemberTypes.Property:
                        member = new PropertyMember(name, (PropertyInfo)field.Info);
                        break;
                    default:
                        throw new NotSupportedException("Only properties and fields are valid serialization members.");
                }

                members.Add(member);
            }
        }

        private static string GetContractName(DataContractAttribute contract, Type t)
        {
            if (string.IsNullOrEmpty(contract.Name))
            {
                return t.FullName;
            }

            return string.IsNullOrEmpty(contract.Namespace) ? contract.Name : contract.Namespace + "." + contract.Name;
        }

        private static string GetMemberName(MemberInfo member, DataMemberAttribute contract)
        {
            if (string.IsNullOrEmpty(contract.Name))
            {
                return member.Name;
            }

            return contract.Name;
        }

        private abstract class SerializationMember
        {
            public abstract void SetValue(object target, object value);
            public abstract object GetValue(object target);

            private readonly string memberName;
            private readonly Type memberType;
            private readonly HessianType hessianType;

            public string MemberName
            {
                get { return memberName; }
            }

            public Type MemberType
            {
                get { return memberType; }
            }

            public HessianType HessianType
            {
                get { return hessianType; }
            }

            protected SerializationMember(string memberName, Type memberType)
            {
                this.memberName = memberName;
                this.memberType = memberType;
                this.hessianType = memberType.GetHessianType();
            }
        }

        private sealed class FieldMember : SerializationMember
        {
            private readonly FieldInfo field;

            public FieldMember(string memberName, FieldInfo field)
                : base(memberName, field.FieldType)
            {
                if (field == null)
                    throw new ArgumentNullException("field");

                this.field = field;
            }

            public override object GetValue(object target)
            {
                if (target == null)
                    throw new ArgumentNullException("target");

                if (field.DeclaringType != target.GetType())
                {
                    throw new TargetException("The given object does not contain the indicated field \"" + field.Name + "\".");
                }

                return field.GetValue(target);
            }

            public override void SetValue(object target, object value)
            {
                if (target == null)
                {
                    throw new ArgumentNullException("target");
                }

                if (field.DeclaringType != target.GetType())
                {
                    throw new TargetException("The given object does not contain the indicated field \"" + field.Name + "\".");
                }

                field.SetValue(target, value);
            }
        }

        private sealed class PropertyMember : SerializationMember
        {
            private readonly PropertyInfo property;

            public PropertyMember(string memberName, PropertyInfo property)
                : base(memberName, property.PropertyType)
            {
                if (property == null)
                    throw new ArgumentNullException("property");

                this.property = property;
            }

            public override object GetValue(object target)
            {
                if (target == null)
                {
                    throw new ArgumentNullException("target");
                }

                return property.GetValue(target, null);
            }

            public override void SetValue(object target, object value)
            {
                if (target == null)
                {
                    throw new ArgumentNullException("target");
                }

                property.SetValue(target, value, null);
            }
        }
    }
}
