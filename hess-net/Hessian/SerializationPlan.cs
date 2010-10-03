using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HessNet.Hessian
{
    public class SerializationPlan
    {
        private static readonly Dictionary<string, SerializationPlan> plans = new Dictionary<string, SerializationPlan>();

        private readonly Type serializedType;

        private SerializationPlan(Type toSerialize)
        {
            if (toSerialize == null)
                throw new ArgumentNullException("toSerialize");

            serializedType = toSerialize;
        }

        public static SerializationPlan Get(Type toSerialize)
        {
            if (plans.ContainsKey(toSerialize.FullName))
                return plans[toSerialize.FullName];

            return plans[toSerialize.FullName] = new SerializationPlan(toSerialize);
        }
    }
}
