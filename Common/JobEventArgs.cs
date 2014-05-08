using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.WordServer.Common
{
    public class JobEventArgs<TTask> : EventArgs
        where TTask : Task
    {
        public LongRunningJobBase Job { get; set; }
        public TTask Task { get; set; }

        public JobEventArgs(LongRunningJobBase job, TTask task)
        {
            this.Job = job;
            this.Task = task;
        }
    }
}
