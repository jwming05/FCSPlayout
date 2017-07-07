using FCSPlayout.WPF.Core;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FCSPlayout.AppInfrastructure
{
    

    public interface IMediaFileImageExtractor
    {
        byte[] GetImageBytes(string filePath, double position, int targetSize);
    }

    public static class MediaFileImageExtractorExtensions
    {
        public static Task<byte[]> GetImageBytesAsync(this IMediaFileImageExtractor extractor,
            string filePath, double position, int targetSize)
        {
            return Task.Run(() => extractor.GetImageBytes(filePath,position,targetSize));
        }
    }

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
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public abstract IntPtr GetHBitmap(string filePath, double position);

        public void GetHBitmapAsync(MediaFileImageRequest request)
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
