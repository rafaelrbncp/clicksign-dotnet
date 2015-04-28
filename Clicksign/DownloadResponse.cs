using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Clicksign
{
    public class DownloadResponse
    {
        private IList<DownloadError> _errors;

        public IList<DownloadError> Errors
        {
            get { return _errors; }
        }


        public bool HasErrors { get { return Errors.Count > 0; } }

        public bool isActionFinished { get { return !HasErrors && binaryFile != null; }}

        public byte[] binaryFile { get; set; }

        public DownloadResponse()
        {
            _errors = new List<DownloadError>();
        }

        public void AddDownloadError(String message)
        {
            _errors.Add(new DownloadError(message,null));
        }

        public void AddDownloadError(String message, Exception exception)
        {
            _errors.Add(new DownloadError(message,exception));
        }


    }
}