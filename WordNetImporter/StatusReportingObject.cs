using System;
using System.Threading;
using System.Threading.Tasks;

namespace WordNetImporter
{
    public abstract class StatusReportingObject
    {
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        protected void RaiseStatusUpdate(StatusCode status, string heading, string details)
        {
            this.OnStatusUpdate(new StatusUpdateEventArgs(status, heading, details));
        }

        protected void RaiseStatusUpdate(Exception error, string heading, string details)
        {
            this.OnStatusUpdate(new StatusUpdateEventArgs(error, heading, details));
        }

        protected virtual void OnStatusUpdate(StatusUpdateEventArgs args)
        {
            if (this.StatusUpdate != null)
                this.StatusUpdate(this, args);
        }
    }

    public abstract class StatusReportingObject<TWorker> : StatusReportingObject
        where TWorker : StatusReportingObject<TWorker>.Worker
    {
        public TaskCreationOptions CreationOptions { get; set; }

        public CancellationTokenSource TokenSource { get; private set; }

        public abstract class Worker : StatusReportingObject
        {
            protected Worker() { }

            public abstract void Invoke();
        }

        protected StatusReportingObject() { }

        protected abstract TWorker CreateWorker();

        public async Task InvokeAsync()
        {
            TWorker worker = this.CreateWorker();
            Task task;
            if (this.TokenSource == null)
                task = new Task(this._Invoke, worker, this.CreationOptions);
            else
                task = new Task(this._Invoke, worker, this.TokenSource.Token, this.CreationOptions);

            task.Start();

            await task;
        }

        private void worker_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            this.OnStatusUpdate(e);
        }

        private void _Invoke(object state)
        {
            Worker worker = state as Worker;

            worker.StatusUpdate += worker_StatusUpdate;
            try
            {
                (state as Worker).Invoke();
            }
            catch
            {
                throw;
            }
            finally
            {
                worker.StatusUpdate -= worker_StatusUpdate;
            }
        }
    }

    public abstract class StatusReportingObject<TWorker, TResults> : StatusReportingObject
        where TWorker : StatusReportingObject<TWorker, TResults>.Worker
    {
        public TaskCreationOptions CreationOptions { get; set; }

        public CancellationTokenSource TokenSource { get; private set; }

        public abstract class Worker : StatusReportingObject
        {
            protected Worker() { }

            public abstract TResults GetResults();
        }

        protected StatusReportingObject() { }

        protected abstract TWorker CreateWorker();

        public async Task<TResults> GetResultsAsync()
        {
            TWorker worker = this.CreateWorker();
            Task<TResults> task;
            if (this.TokenSource == null)
                task = new Task<TResults>(this._GetResults, worker, this.CreationOptions);
            else
                task = new Task<TResults>(this._GetResults, worker, this.TokenSource.Token, this.CreationOptions);

            task.Start();

            TResults results = await task;

            return results;
        }

        private void worker_StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            this.OnStatusUpdate(e);
        }

        private TResults _GetResults(object state)
        {
            Worker worker = state as Worker;

            worker.StatusUpdate += worker_StatusUpdate;
            TResults results;
            try
            {
                results = (state as Worker).GetResults();
            }
            catch
            {
                throw;
            }
            finally
            {
                worker.StatusUpdate -= worker_StatusUpdate;
            }

            return results;
        }
    }
}
