using System;

namespace Clicksign
{
    public class DownloadError
    {
        public DownloadError(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public String Message { get; set; }

        public Exception Exception { get; set; }
    }
}