using FCSPlayout.AppInfrastructure;
using FCSPlayout.WPF.Core;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FCSPlayout.AppInfrastructure
{
    public class MediaFileImageResolver:IMediaFileImageResolver
    {
        private SynchronizationContext _syncContext = SynchronizationContext.Current;

        public void ResolveAsync(IMediaFileImageRequester requester)
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
                    _syncContext.Post(new SendOrPostCallback(SetImageInternal),
                        new Tuple<IMediaFileImageRequester, IntPtr>(requester, ptr));
                }
            };

            MediaFileImageExtractor.Current.GetHBitmapAsync(requester.RequestToken);
        }

        public BitmapSource Resolve(string filePath, double position)
        {
            var ptr = MediaFileImageExtractor.Current.GetHBitmap(filePath, position);
            return ToBitmapSource(ptr);
        }

        private void SetImageInternal(object value)
        {
            Tuple<IMediaFileImageRequester, IntPtr> tuple = (Tuple<IMediaFileImageRequester, IntPtr>)value;

            var ptr = tuple.Item2;
            if (ptr != IntPtr.Zero)
            {
                tuple.Item1.Image = ToBitmapSource(ptr);
            }

            tuple.Item1.RequestToken = null;
        }

        private static BitmapSource ToBitmapSource(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                BitmapSource bmpSource = Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero,
                    Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                
                NativeMethods.DeleteObject(ptr);
                return bmpSource;
            }
            return null;
        }

        public BitmapSource Decode(byte[] imageBytes)
        {
            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                if (decoder.Frames.Count > 0)
                {
                    return decoder.Frames[0];
                }
                return null;
            }
        }
    }
}
