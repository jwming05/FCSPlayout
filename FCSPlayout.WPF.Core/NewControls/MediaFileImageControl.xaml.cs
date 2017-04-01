﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System;

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
            DependencyProperty.Register("MediaFileItem", typeof(BindableMediaFileItem),
                typeof(MediaFileImageControl), new FrameworkPropertyMetadata(null, OnMediaFileItemPropertyChanged));


        public IMediaFileImageResolver MediaFileImageResolver
        {
            get { return (IMediaFileImageResolver)GetValue(MediaFileImageResolverProperty); }
            set { SetValue(MediaFileImageResolverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
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
            ((MediaFileImageControl)dpObj).OnMediaFileItemChanged((BindableMediaFileItem)e.OldValue, (BindableMediaFileItem)e.NewValue);
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

        public BindableMediaFileItem MediaFileItem
        {
            get { return (BindableMediaFileItem)GetValue(MediaFileItemProperty); }
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

        BitmapSource IMediaFileImageRequester.Image
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

                //this.MediaFileItem.StartPosition.TotalSeconds + (this.MediaFileItem.PlayDuration.TotalSeconds / 2.0);
            }
        }

        //byte[] IMediaFileImageRequester.ImageBytes
        //{
        //    get { return this.MediaFileItem.ImageBytes; }
        //}

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
                    //MediaFileImageResolver.Instance.ResolveAsync(this);
                    this.MediaFileImageResolver.ResolveAsync(this);
                }
                else
                {
                    //this.MediaFileItem.Image = MediaFileImageResolver.Decode(this.MediaFileItem.ImageBytes);
                    this.MediaFileItem.Image = this.MediaFileImageResolver.Decode(this.MediaFileItem.ImageBytes);
                }
            }            
        }

        private void OnMediaFileItemChanged(BindableMediaFileItem oldValue, BindableMediaFileItem newValue)
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