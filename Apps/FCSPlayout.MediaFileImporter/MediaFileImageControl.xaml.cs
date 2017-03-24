using FCSPlayout.AppInfrastructure;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System;

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// MediaFileItemControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaFileImageControl : UserControl,IMediaFileImageRequester
    {
        public static readonly DependencyProperty PreviewInteractionRequestProperty =
            DependencyProperty.Register("PreviewInteractionRequest", typeof(InteractionRequest<PlayableItemPreviewNotification>),
                typeof(MediaFileImageControl), new FrameworkPropertyMetadata(null, OnPreviewInteractionRequestPropertyChanged));

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapSource), typeof(MediaFileImageControl),
            new FrameworkPropertyMetadata(null, OnImagePropertyChanged));


        public static readonly DependencyProperty MediaFileItemProperty =
            DependencyProperty.Register("MediaFileItem", typeof(BindableMediaFileItem),
                typeof(MediaFileImageControl), new FrameworkPropertyMetadata(null, OnMediaFileItemPropertyChanged));


        private static void OnMediaFileItemPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MediaFileImageControl)dpObj).OnMediaFileItemChanged((BindableMediaFileItem)e.OldValue, (BindableMediaFileItem)e.NewValue);
        }

        private static void OnImagePropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MediaFileImageControl)dpObj).OnImageChanged((BitmapSource)e.OldValue, (BitmapSource)e.NewValue);
        }

        private static void OnPreviewInteractionRequestPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MediaFileImageControl)dpObj).OnPreviewInteractionRequestChanged((InteractionRequest<PlayableItemPreviewNotification>)e.OldValue, (InteractionRequest<PlayableItemPreviewNotification>)e.NewValue);
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
                return this.MediaFileItem.StartPosition.TotalSeconds + (this.MediaFileItem.PlayDuration.TotalSeconds / 2.0);
            }
        }

        MediaFileImageRequest IMediaFileImageRequester.RequestToken
        {
            get { return _requestToken; }
            set { _requestToken = value; }
        }

        

        private void OnImageChanged(BitmapSource oldValue, BitmapSource newValue)
        {
            if (this.Image == null && this.MediaFileItem != null)
            {
                _requestToken = null;
                MediaFileImageResolver.Instance.Resolve(this);
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

                if(this.MediaFileItem.Image==null && this.Image == null)
                {
                    _requestToken = null;
                    MediaFileImageResolver.Instance.Resolve(this);
                }
            }
            else
            {
                // 这将清除绑定。
                this.Image = null;
            }

            _previewCommand.RaiseCanExecuteChanged();

        }

        private MediaFileImageRequest _requestToken;
        private DelegateCommand _previewCommand;
        private PlayableItemPreviewNotification _previewNotification;

        public MediaFileImageControl()
        {
            InitializeComponent();
            _previewCommand = new DelegateCommand(Preview, CanPreview);
        }

        public ICommand PreviewCommand
        {
            get { return _previewCommand; }
        }

        private bool CanPreview()
        {
            return this.MediaFileItem!=null && this.PreviewInteractionRequest!=null;
        }

        private void Preview()
        {
            if (CanPreview())
            {
                this.PreviewInteractionRequest.Raise(this.PreviewNotification);
            }
        }

        public InteractionRequest<PlayableItemPreviewNotification> PreviewInteractionRequest
        {
            get { return (InteractionRequest<PlayableItemPreviewNotification>)GetValue(PreviewInteractionRequestProperty); }
            set { SetValue(PreviewInteractionRequestProperty, value); }
        }

        private void OnPreviewInteractionRequestChanged(InteractionRequest<PlayableItemPreviewNotification> oldValue, InteractionRequest<PlayableItemPreviewNotification> newValue)
        {
            _previewCommand.RaiseCanExecuteChanged();
        }

        public PlayableItemPreviewNotification PreviewNotification
        {
            get
            {
                if (this.MediaFileItem == null)
                {
                    return null;
                }

                if (_previewNotification == null)
                {
                    _previewNotification = new PlayableItemPreviewNotification();
                }

                if (_previewNotification.PlayableItem != this.MediaFileItem)
                {
                    _previewNotification.PlayableItem = this.MediaFileItem;
                }
                return _previewNotification;
            }
        }
    }
}
