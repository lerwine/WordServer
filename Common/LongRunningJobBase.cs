using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.WordServer.Common
{
    public class LongRunningJobBase
    {
        private static Collection<LongRunningJobBase> _jobs = new Collection<LongRunningJobBase>();
        public static readonly TimeSpan DefaultMaxJobAge = new TimeSpan(0, 5, 0);
        public static TimeSpan MaxJobAge = new TimeSpan(0, 5, 0);
        private Task _bgTask;
        private Task _nestedTask;

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

            LongRunningJobBase[] expired = LongRunningJobBase._jobs.Where(j => j.IsCompleted && j.ExpiresAt < now).ToArray();

            foreach (LongRunningJobBase job in expired)
                LongRunningJobBase._jobs.Remove(job);
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
                        this.OnCompleted(this._nestedTask);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        try
                        {
                            this.Error = this._nestedTask.Exception;
                        }
                        catch { }
                        finally
                        {
                            LongRunningJobBase._FlushJobs();
                            this.ExpiresAt = DateTime.Now.Add((LongRunningJobBase.MaxJobAge > TimeSpan.Zero) ? LongRunningJobBase.MaxJobAge : LongRunningJobBase.DefaultMaxJobAge);
                            this.IsCompleted = true;
                        }
                    }
                }
            }
        }

        protected virtual void OnCompleted(Task task) { }

        public bool Wait(int millisecondsTimeout)
        {
            return (this.IsCompleted || this._bgTask.Wait(millisecondsTimeout));
        }
    }

    public class LongRunningJobBase<TLongRunningJobType, TTaskType> : LongRunningJobBase
        where TLongRunningJobType : LongRunningJobBase<TLongRunningJobType, TTaskType>
        where TTaskType : Task
    {
        public LongRunningJobBase(TTaskType task) : base(task) { }

        public static new TLongRunningJobType GetJob(Guid id)
        {
            return LongRunningJobBase.GetJob<TLongRunningJobType, TTaskType>(id);
        }

        protected virtual void OnCompleted(TTaskType task) { }

        protected override void OnCompleted(Task task)
        {
            base.OnCompleted(task);
            this.OnCompleted(task as TTaskType);
        }
    }
}
