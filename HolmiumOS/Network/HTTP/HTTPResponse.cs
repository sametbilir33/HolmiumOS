using System;
using System.Collections.Generic;

namespace HolmiumOS.Network.HTTP
{
    public sealed class HTTPResponse
    {
        public int StatusCode { get; }
        public string StatusText { get; }
        public Dictionary<string, string> Headers { get; }
        public byte[] Body { get; }

        public HTTPResponse(
            int statusCode,
            string statusText,
            Dictionary<string, string> headers,
            byte[] body)
        {
            StatusCode = statusCode;
            StatusText = statusText;
            Headers = headers;
            Body = body;
        }

        public bool HasHeader(string name)
        {
            return Headers.ContainsKey(name);
        }

        public string? GetHeader(string name)
        {
            if (Headers.TryGetValue(name, out string? value))
                return value;

            return null;
        }
    }
}