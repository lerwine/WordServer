using System.Threading.Tasks;

namespace Erwine.Leonard.T.WordServer.Common
{
    public class JobWaitFinishedEventArgs<TTask> : JobWaitingEventArgs<TTask>
        where TTask : Task
    {
        public bool ExecutionCompleted { get; set; }

        public JobWaitFinishedEventArgs(LongRunningJobBase job, TTask task, int millisecondsTimeout, bool executionCompleted)
            : base(job, task, millisecondsTimeout)
        {
            this.ExecutionCompleted = executionCompleted;
        }
    }
}
