using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HessNet.Text
{
    public class InvalidUnicodeException : ApplicationException
    {
        public InvalidUnicodeException()
            : base("Invalid UTF-8 encoding detected.")
        {

        }
    }
}
