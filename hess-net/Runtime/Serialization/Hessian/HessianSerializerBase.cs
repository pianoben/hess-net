using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel.Channels;

namespace HessNet.Runtime.Serialization.Hessian
{
    public abstract class HessianSerializerBase
    {
        private int typeCount = 0;
        private int objectCount = 0;

        protected readonly Dictionary<int, TypeReference> typesByRef = new Dictionary<int,TypeReference>();
        protected readonly Dictionary<Type, TypeReference> typesByType = new Dictionary<Type, TypeReference>();
        protected readonly Dictionary<int, ObjectReference> objectsByRef = new Dictionary<int,ObjectReference>();
        protected readonly Dictionary<object, ObjectReference> objectsByObj = new Dictionary<object, ObjectReference>();

        protected TypeReference ResolveType(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                throw new ArgumentNullException("typename");

            Type type;

            if ((type = Type.GetType(typename)) == null)
            {
                throw new ArgumentOutOfRangeException("typename", "No type was found for the type named " + typename + ".");
            }

            if (typesByType.ContainsKey(type))
            {
                return typesByType[type];
            }

            var reference = new TypeReference(typeCount++, type);
            
            typesByType[reference.Type] = reference;
            typesByRef[reference.Reference] = reference;

            return reference;
        }

        protected ObjectReference ResolveObject(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (objectsByObj.ContainsKey(obj))
            {
                return objectsByObj[obj];
            }

            return default(ObjectReference);
        }

        protected TypeReference ResolveTypeReference(int reference)
        {
            if (!typesByRef.ContainsKey(reference))
            {
                throw new SerializationException("Invalid Hessian 2.0 type reference.");
            }

            return typesByRef[reference];
        }

        protected ObjectReference ResolveObjectReference(int reference)
        {
            if (!objectsByRef.ContainsKey(reference))
            {
                throw new SerializationException("Invalid Hessian 2.0 object reference.");
            }

            return objectsByRef[reference];
        }

        protected IEnumerable<OutputChunk> ChunkXml(XNode node)
        {
            return ChunkString(node.ToString(SaveOptions.DisableFormatting));
        }

        protected IEnumerable<OutputChunk> ChunkXml(XmlNode node)
        {
            using (var ms = new MemoryStream())
            {
                var writer = XmlWriter.Create(ms, new XmlWriterSettings { Encoding = Encoding.UTF8 });
                node.WriteTo(writer);
                writer.Flush();
                return ChunkBytes(ms.ToArray());
            }
        }

        protected IEnumerable<OutputChunk> ChunkString(string s)
        {
            return ChunkBytes(Encoding.UTF8.GetBytes(s));
        }

        /// <summary>
        /// Converts an array of <see cref="Byte"/>s into a stream of chunks.
        /// </summary>
        /// <remarks>
        /// The field <see cref="OutputChunk.Data"/> of each returned chunk is,
        /// in the current implementation, a reference to a single underlying
        /// buffer.  As such, assigning the Data field to another byte[]
        /// reference will not suffice to save the buffered data;
        /// <see cref="Array.Copy"/> or another method will need to be used.
        /// </remarks>
        /// <param name="data"></param>
        /// <returns></returns>
        protected IEnumerable<OutputChunk> ChunkBytes(byte[] data)
        {
            var length = data.Length;

            // Will the data fit into a single chunk?
            if (length < ushort.MaxValue)
            {
                yield return new OutputChunk
                {
                    IsFinal = true,
                    Length = (ushort)length,
                    Data = data
                };

                yield break;
            }

            var offset = 0;
            var buffer = new byte[ushort.MaxValue];
            
            while (offset < length)
            {
                // Size the data
                var chunkLength = Math.Min(length - offset, ushort.MaxValue);

                // Copy the data to the buffer
                Array.Copy(data, offset, buffer, 0, chunkLength);

                // Create the chunk
                var chunk = new OutputChunk
                {
                    IsFinal = offset + ushort.MaxValue >= data.Length,
                    Length = (ushort)chunkLength,
                    Data = buffer
                };

                // Update the data offset
                offset += chunkLength;

                // Yield the new chunk
                yield return chunk;
            }
        }

        protected struct OutputChunk
        {
            public bool IsFinal;
            public ushort Length;
            public byte[] Data;
        }

        protected struct TypeReference
        {
            public int Reference;
            public Type Type;

            public TypeReference(int reference, Type type)
            {
                Reference = reference;
                Type = type;
            }
        }

        protected struct ObjectReference
        {
            public int Reference;
            public object Object;

            public ObjectReference(int reference, object o)
            {
                Reference = reference;
                Object = o;
            }
        }
    }
}
