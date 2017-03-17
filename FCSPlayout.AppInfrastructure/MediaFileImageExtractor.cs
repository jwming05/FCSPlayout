using System;

namespace FCSPlayout.AppInfrastructure
{
    public abstract class MediaFileImageExtractor : IDisposable
    {
        private static MediaFileImageExtractor _current;

        public static MediaFileImageExtractor Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new MLMediaFileImageExtractor();
                }
                return _current;
            }

            set
            {
                _current = value;
            }
        }


        private AsyncActionExecuter _actionExecuter;

        protected MediaFileImageExtractor()
        {
            _actionExecuter = new AsyncActionExecuter();
            //Common.GlobalEventAggregator.Instance.ApplicationExit += OnApplicationExit;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            this.Dispose();
        }

        protected abstract IntPtr GetHBitmap(string filePath, double position);

        public void GetHBitmap(MediaFileImageRequest request)
        {
            _actionExecuter.Add(() =>
            {
                if (!request.Cancel)
                {
                    IntPtr result = GetHBitmap(request.Path, request.Position);
                    request.Complete(result);
                }
            });
        }

        public virtual void Dispose()
        {
            if (_actionExecuter != null)
            {
                _actionExecuter.Dispose();
                _actionExecuter = null;
            }
        }
    }

    
}
