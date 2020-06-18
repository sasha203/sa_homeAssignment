using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CustomException : Exception
    {
        //microsoft suggest 3 constructors.
        public CustomException() : base() { }
        public CustomException(string message) : base(message) { } //the most used constructor.
        public CustomException(string message, Exception innerException) : base(message, innerException) { }
    }
}
