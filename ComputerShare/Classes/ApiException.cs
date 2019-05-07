using System;

namespace ComputerShare.Classes
{
    public class ApiException : Exception
    {
        public int StatusCode { get; private set; }

        public string StringContent { get; private set; }

        public byte[] BytesContent { get; private set; }

        public ApiException(string message, int statusCode, string stringContent)
            : base(message)
        {
            StatusCode = statusCode;
            StringContent = stringContent;
        }
    }
}