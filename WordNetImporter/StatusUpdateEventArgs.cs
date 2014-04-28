using System;

namespace WordNetImporter
{
    public class StatusUpdateEventArgs : EventArgs
    {
        public StatusCode Status { get; private set; }
        public string Heading { get; private set; }
        public string Details { get; private set; }
        public Exception Error { get; private set; }
        public StatusUpdateEventArgs(StatusCode status, string heading, string details) : this(status, null, heading, details) { }
        public StatusUpdateEventArgs(Exception error, string heading, string details) : this(StatusCode.Error, error, heading, details) { }
        public StatusUpdateEventArgs(StatusCode status, Exception error, string heading, string details)
        {
            this.Status = status;
            this.Error = error;
            this.Heading = heading;
            this.Details = details;
        }
    }
}
