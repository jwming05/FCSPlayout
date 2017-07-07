using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// MediaFileItemControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaFileImageControl : UserControl,IMediaFileImageRequester
    {
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapSource), typeof(MediaFileImageControl),
            new FrameworkPropertyMetadata(null, OnImagePropertyChanged));


        public static readonly DependencyProperty MediaFileItemProperty =
            DependencyProperty.Register("MediaFileItem", typeof(IImageItem),
                typeof(MediaFileImageControl), new FrameworkPropertyMetadata(null, OnMediaFileItemPropertyChanged));


        public IMediaFileImageResolver MediaFileImageResolver
        {
            get { return (IMediaFileImageResolver)GetValue(MediaFileImageResolverProperty); }
            set { SetValue(MediaFileImageResolverProperty, value); }
        }

        public static readonly DependencyProperty MediaFileImageResolverProperty =
            DependencyProperty.Register("MediaFileImageResolver", typeof(IMediaFileImageResolver), typeof(MediaFileImageControl), 
                new FrameworkPropertyMetadata(null, OnMediaFileImageResolverPropertyChanged));

        private static void OnMediaFileImageResolverPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MediaFileImageControl)dpObj).ResolveImage();
        }

        //private void OnMediaFileImageResolverChanged(IMediaFileImageResolver oldValue, IMediaFileImageResolver newValue)
        //{
        //    
        //}

        private static void OnMediaFileItemPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MediaFileImageControl)dpObj).OnMediaFileItemChanged((IImageItem)e.OldValue, (IImageItem)e.NewValue);
        }

        private static void OnImagePropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MediaFileImageControl)dpObj).OnImageChanged((BitmapSource)e.OldValue, (BitmapSource)e.NewValue);
        }

        private MediaFileImageRequest _requestToken;

        public MediaFileImageControl()
        {
            InitializeComponent();
        }

        public IImageItem MediaFileItem
        {
            get { return (IImageItem)GetValue(MediaFileItemProperty); }
            set { SetValue(MediaFileItemProperty, value); }
        }

        public BitmapSource Image
        {
            get { return (BitmapSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        string IMediaFileImageRequester.FilePath
        {
            get
            {
                return this.MediaFileItem.FilePath;
            }
        }

        ImageSource IMediaFileImageRequester.Image
        {
            get
            {
                return this.MediaFileItem.Image;
            }

            set
            {
                this.MediaFileItem.Image=value;
            }
        }

        double IMediaFileImageRequester.Position
        {
            get
            {
                return this.MediaFileItem.Duration.TotalSeconds/2.0;
            }
        }

        MediaFileImageRequest IMediaFileImageRequester.RequestToken
        {
            get { return _requestToken; }
            set { _requestToken = value; }
        }

        

        private void OnImageChanged(BitmapSource oldValue, BitmapSource newValue)
        {
            ResolveImage();
        }

        private void ResolveImage()
        {
            if (this.MediaFileItem != null && this.MediaFileItem.Image == null && this.Image == null && this.MediaFileImageResolver != null)
            {
                if (this.MediaFileItem.ImageBytes == null)
                {
                    _requestToken = null;
                    this.MediaFileImageResolver.ResolveAsync(this);
                }
                else
                {
                    this.MediaFileItem.Image = this.MediaFileImageResolver.Decode(this.MediaFileItem.ImageBytes);
                }
            }            
        }

        private void OnMediaFileItemChanged(IImageItem oldValue, IImageItem newValue)
        {
            if (this.MediaFileItem != null)
            {
                var binding = new Binding("Image");
                binding.Source = this.MediaFileItem;
                binding.Mode = BindingMode.OneWay;
                this.SetBinding(ImageProperty, binding);

                ResolveImage();
            }
            else
            {
                // 这将清除绑定。
                this.Image = null;
            }
        }
    }
}
