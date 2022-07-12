using System;

namespace SonarWarnings
{
    public class Error
    {
        public string StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

    }
}