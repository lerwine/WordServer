using System;

namespace Erwine.Leonard.T.WordServer.Common
{
    public class JobsFlushedEventArgs : EventArgs
    {
        public LongRunningJobBase[] ExpiredJobs { get; set; }
        public LongRunningJobBase[] CurrentJobs { get; set; }

        public JobsFlushedEventArgs(LongRunningJobBase[] expiredJobs, LongRunningJobBase[] currentJobs)
        {
            this.ExpiredJobs = expiredJobs;
            this.ExpiredJobs = currentJobs;
        }
    }
}
