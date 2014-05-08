using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.WordServer.Common
{
    public class LongRunningJobBase
    {
        private static Collection<LongRunningJobBase> _jobs = new Collection<LongRunningJobBase>();

        private Task _bgTask;
        private Task _nestedTask;

        public static event EventHandler<JobsFlushingEventArgs> JobsFlushing;
        public static event EventHandler<JobsFlushedEventArgs> JobsFlushed;
        public event EventHandler<JobEventArgs<Task>> JobCompleting;
        public event EventHandler<JobEventArgs<Task>> JobCompleted;

        public static readonly TimeSpan DefaultMaxJobAge = new TimeSpan(0, 5, 0);
        public static TimeSpan MaxJobAge = new TimeSpan(0, 5, 0);

        public Guid ID { get; private set; }
        public bool IsCompleted { get; private set; }
        public Exception Error { get; private set; }
        public DateTime ExpiresAt { get; set; }

        public static TLongRunningJobType GetJob<TLongRunningJobType, TTaskType>(Guid id)
            where TLongRunningJobType : LongRunningJobBase<TLongRunningJobType, TTaskType>
            where TTaskType : Task
        {
            TLongRunningJobType result;

            lock (LongRunningJobBase._jobs)
            {
                LongRunningJobBase._FlushJobs();
                result = LongRunningJobBase._jobs.OfType<TLongRunningJobType>().FirstOrDefault(j => j.ID.Equals(id));
            }

            return result;
        }

        protected LongRunningJobBase(Task task)
        {
            this.ID = Guid.NewGuid();
            this._nestedTask = task;
            this._bgTask = new Task(this._Worker);
            lock (LongRunningJobBase._jobs)
            {
                LongRunningJobBase._FlushJobs();
                LongRunningJobBase._jobs.Add(this);
            }
        }

        public static void FlushJobs()
        {
            lock (LongRunningJobBase._jobs)
            {
                LongRunningJobBase._FlushJobs();
            }
        }

        private static void _FlushJobs()
        {
            DateTime now = DateTime.Now;

            LongRunningJobBase[] expired = LongRunningJobBase.RaiseJobsFlushing(LongRunningJobBase._jobs.ToArray(), now);

            foreach (LongRunningJobBase job in expired)
                LongRunningJobBase._jobs.Remove(job);

            LongRunningJobBase.RaiseJobsFlushed(expired, LongRunningJobBase._jobs.ToArray());
        }

        private static LongRunningJobBase[] RaiseJobsFlushing(LongRunningJobBase[] allJobs, DateTime expirationTimeStamp)
        {
            JobsFlushingEventArgs args = new JobsFlushingEventArgs(allJobs, expirationTimeStamp);
            LongRunningJobBase.OnJobsFlushing(args);
            return args.ExpiredJobs;
        }

        protected virtual static void OnJobsFlushing(JobsFlushingEventArgs args)
        {
            args.ExpiredJobs = LongRunningJobBase._jobs.Where(j => j.IsCompleted && j.ExpiresAt < args.ExpirationTimeStamp).ToArray();
            if (LongRunningJobBase.JobsFlushing != null)
                LongRunningJobBase.JobsFlushing(null, args);
        }

        private static void RaiseJobsFlushed(LongRunningJobBase[] expiredJobs, LongRunningJobBase[] currentJobs)
        {
            LongRunningJobBase.OnJobsFlushed(new JobsFlushedEventArgs(expiredJobs, currentJobs));
        }

        protected virtual static void OnJobsFlushed(JobsFlushedEventArgs args)
        {
            if (LongRunningJobBase.JobsFlushed != null)
                LongRunningJobBase.JobsFlushed(null, args);
        }

        private void _Worker()
        {
            try
            {
                this._nestedTask.Wait();
            }
            catch
            {
                throw;
            }
            finally
            {
                lock (LongRunningJobBase._jobs)
                {
                    try
                    {
                        this.RaiseJobCompleting();
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        this.RaiseJobCompleted();
                    }
                }
            }
        }

        private void RaiseJobCompleting()
        {
            this.OnJobCompleting(new JobEventArgs<Task>(this, this._nestedTask));
        }

        protected virtual void OnJobCompleting(JobEventArgs<Task> args)
        {
            if (this.JobCompleting != null)
                this.JobCompleting(this, args);
        }

        private void RaiseJobCompleted()
        {
            this.OnJobCompleted(new JobEventArgs<Task>(this, this._nestedTask));
        }

        protected virtual void OnJobCompleted(JobEventArgs<Task> args)
        {
            try
            {
                this.Error = args.Task.Exception;
            }
            catch { }
            finally
            {
                LongRunningJobBase._FlushJobs();
                args.Job.ExpiresAt = DateTime.Now.Add((LongRunningJobBase.MaxJobAge > TimeSpan.Zero) ? LongRunningJobBase.MaxJobAge : LongRunningJobBase.DefaultMaxJobAge);
                args.Job.IsCompleted = true;
                if (this.JobCompleted != null)
                    this.JobCompleted(this, args);
            }
        }

        public event EventHandler<JobWaitingEventArgs<Task>> JobWaiting;
        public event EventHandler<JobWaitFinishedEventArgs<Task>> JobWaitFinished;

        public bool Wait(int millisecondsTimeout)
        {
            if (this.IsCompleted)
                return true;

            millisecondsTimeout = this.RaiseJobWaiting(millisecondsTimeout);
            return this.RaiseJobWaitFinished(millisecondsTimeout, this._bgTask.Wait(args.MillisecondsTimeout));
        }

        private int RaiseJobWaiting(int millisecondsTimeout)
        {
            JobWaitingEventArgs<Task> args = new JobWaitingEventArgs<Task>(this, this._nestedTask, millisecondsTimeout);
            this.OnJobWaiting(args);
            return args.MillisecondsTimeout;
        }

        protected virtual void OnJobWaiting(JobWaitingEventArgs<Task> args)
        {
            if (this.JobWaiting != null)
                this.JobWaiting(this, args);
        }

        private bool RaiseJobWaitFinished(int millisecondsTimeout, bool executionCompleted)
        {
            JobWaitFinishedEventArgs<Task> args = new JobWaitFinishedEventArgs<Task>(this, this._nestedTask, millisecondsTimeout, executionCompleted);
            this.OnJobWaitFinished(args);
            return args.ExecutionCompleted;
        }

        private void OnJobWaitFinished(JobWaitFinishedEventArgs<Task> args)
        {
            if (this.JobWaitFinished != null)
                this.JobWaitFinished(this, args);
        }
    }

    public class LongRunningJobBase<TLongRunningJobType, TTaskType> : LongRunningJobBase
        where TLongRunningJobType : LongRunningJobBase<TLongRunningJobType, TTaskType>
        where TTaskType : Task
    {
        public LongRunningJobBase(TTaskType task) : base(task) { }

        public static TLongRunningJobType GetJob(Guid id)
        {
            return LongRunningJobBase.GetJob<TLongRunningJobType, TTaskType>(id);
        }

        protected virtual void OnCompleted(TTaskType task) { }

        protected virtual void OnJobCompleted(JobEventArgs<TTaskType> args)
        {
            this.OnCompleted(args.Task);
        }

        protected override void OnJobCompleted(JobEventArgs<Task> args)
        {
            base.OnJobCompleted(args);
            JobEventArgs<TTaskType> args2 = new JobEventArgs<TTaskType>(args.Job, args.Task as TTaskType);
            this.OnJobCompleted(args2);
            args.Job = args2.Job;
            args.Task = args2.Task;
        }
    }
}
