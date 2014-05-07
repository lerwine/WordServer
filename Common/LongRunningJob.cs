using System.Threading.Tasks;

namespace Erwine.Leonard.T.WordServer.Common
{
    public class LongRunningJob : LongRunningJobBase<LongRunningJob, Task>
    {
        public LongRunningJob(Task task) : base(task) { }
    }

    public class LongRunningJob<TResult> : LongRunningJobBase<LongRunningJob<TResult>, Task<TResult>>
    {
        public TResult Result { get; private set; }

        public LongRunningJob(Task<TResult> task) : base(task) { }

        protected override void OnCompleted(Task<TResult> task)
        {
            base.OnCompleted(task);
            this.Result = task.Result;
        }
    }
}
