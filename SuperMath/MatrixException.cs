using System;
using System.Runtime.Serialization;

namespace SuperMath
{
    [Serializable]
    internal class MatrixException : Exception
    {
        public MatrixException()
        {
        }

        public MatrixException(string message) : base(message)
        {
        }

        public MatrixException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MatrixException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}