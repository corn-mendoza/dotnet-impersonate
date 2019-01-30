using System;

namespace BinaryBootstrapper
{
    public class ValidationException : Exception
    {
        
        public ValidationException(string message) : base(message)
        {
        }
        
    }
}
