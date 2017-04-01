using FCSPlayout.Domain;
using System.Windows;
using System.Windows.Controls;
using System;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// MediaItemSearchOptionControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaItemSearchOptionControl : UserControl
    {
        public MediaItemSearchOptions SearchOptions
        {
            get { return (MediaItemSearchOptions)GetValue(SearchOptionsProperty); }
            set { SetValue(SearchOptionsProperty, value); }
        }

        public static readonly DependencyProperty SearchOptionsProperty =
            DependencyProperty.Register("SearchOptions", typeof(MediaItemSearchOptions), typeof(MediaItemSearchOptionControl), 
                new FrameworkPropertyMetadata(null, OnSearchOptionsPropertyChanged));
        private BindableMediaItemSearchOptions _bindableSearchOptions;

        private static void OnSearchOptionsPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((MediaItemSearchOptionControl)dpObj).OnSearchOptionsChanged((MediaItemSearchOptions)e.OldValue,(MediaItemSearchOptions)e.NewValue);
        }

        private void OnSearchOptionsChanged(MediaItemSearchOptions oldValue, MediaItemSearchOptions newValue)
        {
            if (this.SearchOptions != null)
            {
                this.BindableSearchOptions.Options = this.SearchOptions;
            }
        }

        public MediaItemSearchOptionControl()
        {
            InitializeComponent();
        }

        public BindableMediaItemSearchOptions BindableSearchOptions
        {
            get
            {
                if (_bindableSearchOptions == null)
                {
                    _bindableSearchOptions = new BindableMediaItemSearchOptions();
                }
                return _bindableSearchOptions;
            }
        }
    }
}
