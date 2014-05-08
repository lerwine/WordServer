using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.WordServer.Common
{
    public class JobWaitingEventArgs<TTask> : JobEventArgs<TTask>
        where TTask : Task
    {
        public int MillisecondsTimeout { get; set; }

        public JobWaitingEventArgs(LongRunningJobBase job, TTask task, int millisecondsTimeout)
            : base(job, task)
        {
            this.MillisecondsTimeout = millisecondsTimeout;
        }
    }
}
