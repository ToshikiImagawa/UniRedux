using System;

namespace UniRedux
{
    [System.Diagnostics.DebuggerStepThrough]
    public class ReduxException : Exception
    {
        public ReduxException(string message) : base(message)
        {
        }

        public ReduxException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}