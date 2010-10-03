using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace HessNet.Runtime.Serialization.Hessian
{
    public class DataContractHessianSerializer : XmlObjectSerializer
    {
        const string root = "root";

        public static Dictionary<Type, DataContractHessianSerializer> Serializers = new Dictionary<Type, DataContractHessianSerializer>();

        public DataContractHessianSerializer(Type contract)
        {

        }

        public override bool IsStartObject(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            reader.MoveToContent();
            return reader.IsStartElement(root, string.Empty);
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            return IsStartObject((XmlReader)reader);
        }

        public override object ReadObject(XmlReader reader, bool verifyObjectName)
        {
            return base.ReadObject(reader, verifyObjectName);
        }

        public override object ReadObject(Stream stream)
        {
            return base.ReadObject(stream);
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            throw new NotImplementedException();
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            throw new NotImplementedException();
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            throw new NotImplementedException();
        }
    }
}
