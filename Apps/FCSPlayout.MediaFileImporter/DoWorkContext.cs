using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    //public class DelegateBackgroundWorker<TArgument, TResult>
    //{

    //    private BackgroundWorker _worker;

    //    public DelegateBackgroundWorker()
    //    {
    //        _worker = new BackgroundWorker();
    //        _worker.WorkerReportsProgress = true;
    //        _worker.WorkerSupportsCancellation = true;
    //        _worker.DoWork += Worker_DoWork;
    //        _worker.ProgressChanged += Worker_ProgressChanged;
    //        _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
    //    }

    //    public void Run(Func<DoWorkContext, bool> doWorkHandler)  // 无参数，无返回值
    //    {

    //    }

    //    public void Run(TArgument argument)  // 有参数，无返回值
    //    {

    //    }

    //    public void Run()  // 无参数，有返回值
    //    {

    //    }

    //    public void Run(TArgument argument)  // 有参数，有返回值
    //    {

    //    }

    //    public void Run(TArgument argument,
    //        Func<DoWorkContext<TArgument, TResult>, bool> doWorkHandler)
    //    {
    //        var context = new DoWorkContext<TArgument, TResult>(argument, _worker);
    //        context.DoWorkHandler = doWorkHandler;
    //        _worker.RunWorkerAsync(context);
    //    }

    //    public void Cancel()
    //    {
    //        _worker.CancelAsync();
    //    }

    //    //public Func<DoWorkContext<TResult>, bool> DoWorkHandler { get; set; }

    //    public Action<int> ProgressChangedHandler { get; set; }
    //    public Action<Exception> ErrorHandler { get; set; }
    //    public Action CancelledHandler { get; set; }
    //    public Action<TResult> SuccessHandler { get; set; }

    //    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    //    {
    //        DoWorkContext<TArgument, TResult> context = (DoWorkContext<TArgument, TResult>)e.Argument;
    //        if (!context.DoWorkHandler(context))
    //        {
    //            e.Cancel = true;
    //        }
    //        else
    //        {
    //            e.Result = context.Result;
    //        }
    //    }

    //    private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    //    {

    //        if (this.ProgressChangedHandler != null)
    //        {
    //            this.ProgressChangedHandler(e.ProgressPercentage);
    //        }
    //    }

    //    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    //    {
    //        if (e.Error != null)
    //        {
    //            if (this.ErrorHandler != null)
    //            {
    //                this.ErrorHandler(e.Error);
    //            }
    //        }
    //        else if (e.Cancelled)
    //        {
    //            if (this.CancelledHandler != null)
    //            {
    //                this.CancelledHandler();
    //            }
    //        }
    //        else
    //        {
    //            if (this.SuccessHandler != null)
    //            {
    //                this.SuccessHandler((TResult)e.Result);
    //            }
    //        }


    //    }
    //}

    //public class DoWorkContext
    //{
    //    private BackgroundWorker _worker;

    //    protected internal DoWorkContext(BackgroundWorker worker)
    //    {
    //        _worker = worker;
    //    }

    //    public void SetProgress(int progress)
    //    {
    //        _worker.ReportProgress(progress);
    //    }

    //    protected internal BackgroundWorker Worker
    //    {
    //        get
    //        {
    //            return _worker;
    //        }
    //    }
    //}

    //public class DoWorkContext<TArgument> : DoWorkContext
    //{
    //    protected internal DoWorkContext(BackgroundWorker worker) : base(worker)
    //    {
    //    }

    //    protected internal DoWorkContext(TArgument argument, BackgroundWorker worker) : this(worker)
    //    {
    //        this.Argument = argument;
    //    }

    //    public TArgument Argument { get; set; }
    //}

    //public class DoWorkContext<TArgument, TResult> : DoWorkContext<TArgument>
    //{
    //    protected internal DoWorkContext(BackgroundWorker worker) : base(worker)
    //    {
    //    }

    //    protected internal DoWorkContext(TArgument argument, BackgroundWorker worker) : base(argument, worker)
    //    {
    //    }

    //    public TResult Result { get; set; }

    //    internal Func<DoWorkContext<TArgument, TResult>, bool> DoWorkHandler { get; set; }
    //}
}
