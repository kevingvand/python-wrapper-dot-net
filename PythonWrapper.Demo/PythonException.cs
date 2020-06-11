using System;

namespace PythonWrapper
{
    public class PythonException : Exception
    {
        public PythonException(string message)
            : base(message)
        {
        }
    }
}
