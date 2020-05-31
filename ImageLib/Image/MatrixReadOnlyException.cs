using System;
using System.Collections.Generic;
using System.Text;

namespace ImageLib.Image
{
    public class MatrixReadOnlyException : Exception
    {
        public MatrixReadOnlyException() : base()
        {

        }
        public MatrixReadOnlyException(string message) : base(message)
        {
        }

        public MatrixReadOnlyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
