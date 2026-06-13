using System;

namespace Produkty24_Web
{
    public class ApiException : Exception
    {
        public ApiException(string message)
            : base(message)
        {
        }
    }
}
