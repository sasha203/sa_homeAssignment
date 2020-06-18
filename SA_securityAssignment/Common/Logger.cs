using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Logger {

        public static void Log(string user, string methodName, string message) {
            Trace.WriteLine(String.Format("Date: {0}, User: {1}, Method: {2}, Message: {3} ",
                DateTime.Now.ToString(), user, methodName, message));
        }

        public static void Log(string user, string methodName, string message, string additionalMessage)
        {
            Trace.WriteLine(String.Format("Date: {0}, User: {1}, Method: {2}, Message: {3}, Message #2: {4} ",
                DateTime.Now.ToString(), user, methodName, message, additionalMessage));
        }
    }
}
