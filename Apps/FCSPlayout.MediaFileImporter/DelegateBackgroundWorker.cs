using FCSPlayout.AppInfrastructure;
using System;
using System.ComponentModel;

namespace FCSPlayout.WPFApp.ViewModels
{
    internal class DelegateBackgroundWorker
    {
        private BackgroundWorker _worker;
        private bool _running;

        public DelegateBackgroundWorker()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;

            _worker.DoWork += OnDoWork;
            _worker.ProgressChanged += OnProgressChanged;
            _worker.RunWorkerCompleted += OnRunWorkerCompleted;
        }

        public Action<int,object> ProgressChangedHandler { get; set; }

        public Action<Exception,bool,object> RunCompletedHandler { get; set; }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _running = false;

            if (this.RunCompletedHandler != null)
            {
                this.RunCompletedHandler(e.Error, e.Cancelled, e.Result);
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.ProgressChangedHandler != null)
            {
                this.ProgressChangedHandler(e.ProgressPercentage,e.UserState);
            }
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            var context = (WorkContext)e.Argument;
            context.Action(context);
        }

        public void Run(Action<IBackgroundWorkContext> action)
        {
            if (_running) throw new InvalidOperationException();
            _running = true;
            _worker.RunWorkerAsync(new WorkContext(_worker) { Action=action });
        }

        public object State { get; set; }

        class WorkContext : IBackgroundWorkContext
        {
            private BackgroundWorker _worker;

            public WorkContext(BackgroundWorker worker)
            {
                _worker = worker;
            }

            internal Action<IBackgroundWorkContext> Action { get; set; }

            public void SetProgress(int progress)
            {
                _worker.ReportProgress(progress);
            }

            public void SetProgress(int progress, object state)
            {
                _worker.ReportProgress(progress, state);
            }
        }
    }

    
}