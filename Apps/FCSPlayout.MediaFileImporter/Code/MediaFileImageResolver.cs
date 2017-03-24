using FCSPlayout.AppInfrastructure;
using System;
using System.Threading;
using System.Windows.Media.Imaging;

namespace FCSPlayout.MediaFileImporter
{
    class MediaFileImageResolver
    {
        private SynchronizationContext _syncContext = SynchronizationContext.Current;

        private static Lazy<MediaFileImageResolver> _instance=new Lazy<MediaFileImageResolver>(()=>new MediaFileImageResolver(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        
        internal static MediaFileImageResolver Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private MediaFileImageResolver()
        {

        }
        public void Resolve(IMediaFileImageRequester requester)
        {
            if (requester.RequestToken != null) return;

            string file = requester.FilePath;
            double pos = requester.Position;

            requester.RequestToken = new MediaFileImageRequest
            {
                Path = file,
                Position = pos,
                Complete = (ptr) =>
                {
                    _syncContext.Post(new System.Threading.SendOrPostCallback(SetImageInternal), 
                        new Tuple<IMediaFileImageRequester,IntPtr>(requester, ptr));
                }
            };

            MediaFileImageExtractor.Current.GetHBitmap(requester.RequestToken);
        }

        private void SetImageInternal(object value)
        {
            Tuple<IMediaFileImageRequester, IntPtr> tuple = (Tuple<IMediaFileImageRequester, IntPtr>)value;

            var ptr = tuple.Item2;
            if (ptr != IntPtr.Zero)
            {
                BitmapSource bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero,
                System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                NativeMethods.DeleteObject(ptr);
                tuple.Item1.Image = bmpSource;
            }
            tuple.Item1.RequestToken = null;
        }
    }
}
