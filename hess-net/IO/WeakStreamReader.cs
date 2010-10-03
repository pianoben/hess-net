using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HessNet.IO
{
    public class WeakStreamReader : StreamReader
    {
        public WeakStreamReader(Stream stream)
            : base(new WeakStream(stream), Encoding.UTF8)
        {

        }
    }
}
