using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace HessNet.ServiceModel.Dispatcher
{
    public class HessianFormatter : IDispatchMessageFormatter, IClientMessageFormatter
    {
        public void DeserializeRequest(Message message, object[] parameters)
        {
            
            throw new NotImplementedException();
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            throw new NotImplementedException();
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
