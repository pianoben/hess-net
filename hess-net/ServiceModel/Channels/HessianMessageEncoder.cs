using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace HessNet.ServiceModel.Channels
{
    public class HessianMessageEncoder : MessageEncoder
    {
        public override string ContentType
        {
            get { return "application/x-hessian"; }
        }

        public override string MediaType
        {
            get { return null; }
        }

        public override MessageVersion MessageVersion
        {
            get { return MessageVersion.None; }
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            throw new NotImplementedException();
        }

        public override Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType)
        {
            throw new NotImplementedException();
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            throw new NotImplementedException();
        }

        public override void WriteMessage(Message message, System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
