using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    public class AsyncActionExecuter : IDisposable
    {
        private BlockingCollection<Action> _actions;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;
        private bool _disposed = false;

        public AsyncActionExecuter()
        {
            _actions = new BlockingCollection<Action>();
            _cancellationTokenSource = new CancellationTokenSource();
            _task = Task.Factory.StartNew(Execute);
        }

        private void Execute()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var action = _actions.Take();
                try
                {
                    action();
                }
                catch (Exception ex)
                {

                }
            }
        }
        public void Add(Action action)
        {
            if (!_disposed && !_cancellationTokenSource.IsCancellationRequested)
            {
                _actions.Add(action);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _cancellationTokenSource.Cancel();

                _actions.Add(() => { });
                _task.Wait();

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                _task.Dispose();
                _task = null;

                _actions.Dispose();
                _actions = null;
            }

        }
    }
}