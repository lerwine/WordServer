using System;

namespace Erwine.Leonard.T.WordServer.Common
{
    public class JobsFlushingEventArgs : EventArgs
    {
        public LongRunningJobBase[] AllJobs { get; set; }
        public DateTime ExpirationTimeStamp { get; set; }
        public LongRunningJobBase[] ExpiredJobs { get; set; }

        public JobsFlushingEventArgs(LongRunningJobBase[] allJobs, DateTime expirationTimeStamp)
        {
            this.AllJobs = allJobs;
            this.ExpirationTimeStamp = expirationTimeStamp;
        }
    }
}
