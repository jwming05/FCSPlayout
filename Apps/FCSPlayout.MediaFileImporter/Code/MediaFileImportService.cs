using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    public class MediaFileImportService
    {
        private BackgroundWorker _worker;
        private RunContext _runContext;
        private WorkerToken _token;

        public MediaFileImportService()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        public IWorkerToken Start(RunContext runContext)
        {
            if (runContext == null)
            {
                throw new ArgumentNullException();
            }

            if (_runContext != null)
            {
                throw new InvalidOperationException();
            }

            _runContext = runContext;
            _worker.RunWorkerAsync();
            _token = new WorkerToken(_worker);
            return _token;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _runContext = null;
            _token.Worker = null;
            _token = null;

            if (e.Error != null)
            {
                if (_runContext.OnError != null)
                {
                    _runContext.OnError();
                }
            }
            else if (e.Cancelled)
            {
                if (_runContext.OnCancelled != null)
                {
                    _runContext.OnCancelled();
                }
            }
            else
            {
                if (_runContext.OnSuccess != null)
                {
                    _runContext.OnSuccess(e.Result);
                }
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_runContext.ProgressChangedAction != null)
            {
                _runContext.ProgressChangedAction(e.ProgressPercentage, e.UserState);
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Copy(_runContext.Source, _runContext.Destination);
        }

        private void Copy(Stream source, Stream destination)
        {
            byte[] buffer = new byte[1024];
            long length = source.Length;
            int count = source.Read(buffer, 0, buffer.Length);
            int totalCount = 0;
            double percent = 0.0;
            while (count > 0)
            {
                destination.Write(buffer, 0, count);

                totalCount += count;
                percent = ((double)totalCount) / ((double)length);
                _worker.ReportProgress((int)percent);
                count = source.Read(buffer, 0, buffer.Length);
            }
        }

        public class RunContext
        {
            public Stream Destination { get; set; }
            public Stream Source { get; set; }
            public Action<int, object> ProgressChangedAction { get; set; }
            public Action OnError { get; set; }

            public Action OnCancelled { get; set; }

            public Action<object> OnSuccess { get; set; }
        }

        class WorkerToken : IWorkerToken
        {
            private BackgroundWorker _worker;

            internal BackgroundWorker Worker
            {
                get
                {
                    return _worker;
                }

                set
                {
                    _worker = value;
                }
            }

            public void CancelAsync()
            {
                if (_worker != null)
                {
                    _worker.CancelAsync();
                }
            }

            public WorkerToken(BackgroundWorker _worker)
            {
                this._worker = _worker;
            }
        }
    }

    public interface IWorkerToken
    {
        void CancelAsync();
    }
}
