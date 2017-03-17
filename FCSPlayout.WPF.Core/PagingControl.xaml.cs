using FCSPlayout.Domain;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// PagingControl.xaml 的交互逻辑
    /// </summary>
    public partial class PagingControl : UserControl
    {
        public PagingItems PagingItems
        {
            get { return (PagingItems)GetValue(PagingItemsProperty); }
            set { SetValue(PagingItemsProperty, value); }
        }

        public static readonly DependencyProperty PagingItemsProperty =
            DependencyProperty.Register("PagingItems", typeof(PagingItems), typeof(PagingControl), 
                new FrameworkPropertyMetadata(null, OnPagingItemsPropertyChanged));

        private static void OnPagingItemsPropertyChanged(DependencyObject dpObj,
            DependencyPropertyChangedEventArgs e)
        {
            ((PagingControl)dpObj).OnPagingItemsChanged((PagingItems)e.OldValue, (PagingItems)e.NewValue);
        }

        private void OnPagingItemsChanged(PagingItems oldValue, PagingItems newValue)
        {
            if(this.PagingItems==null || this.PagingItems.RowCount < 1)
            {
                this.panel.IsEnabled = false;

                this.lblInfo.Text = string.Format("第{0}页/共{1}页", 0, 0);

                this.iudPageIndex.Value = 1;
            }
            else
            {
                this.panel.IsEnabled = true;

                this.iudPageIndex.Maximum = this.PagingItems.PageCount;

                this.btnPrev.IsEnabled = this.btnFirst.IsEnabled = this.PagingItems.PageIndex > 0;
                this.btnNext.IsEnabled = this.btnLast.IsEnabled = this.PagingItems.PageIndex <this.PagingItems.PageCount-1;

                this.lblInfo.Text = string.Format("第{0}页/共{1}页",this.PagingItems.PageIndex+1,this.PagingItems.PageCount);             
                this.iudPageIndex.Value = this.PagingItems.PageIndex + 1;
            }
        }

        public static readonly DependencyProperty RowsPerPageProperty =
            DependencyProperty.Register("RowsPerPage", typeof(int), typeof(PagingControl), 
                new FrameworkPropertyMetadata(1), ValidateRowsPerPage);

        private static bool ValidateRowsPerPage(object value)
        {
            return ((int)value) >= 1;
        }

        public event EventHandler<RequestPagingItemsEventArgs> RequestPagingItems;
        public PagingControl()
        {
            InitializeComponent();
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            if(this.PageCount>0 && this.PageIndex>0)
            {
                OnRequestPagingItems(0);
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (this.PageCount > 0 && this.PageIndex > 0)
            {
                OnRequestPagingItems(this.PageIndex-1);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (this.PageCount > 0 && this.PageIndex < this.PageCount-1)
            {
                OnRequestPagingItems(this.PageIndex + 1);
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            if (this.PageCount > 0 && this.PageIndex < this.PageCount - 1)
            {
                OnRequestPagingItems(this.PageCount - 1);
            }
        }
        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            var pageIndex = iudPageIndex.Value.Value-1;
            if (this.PageCount > 0 && pageIndex < this.PageCount && pageIndex>=0)
            {
                OnRequestPagingItems(pageIndex);
            }
        }

        private void OnRequestPagingItems(int pageIndex)
        {
            if (this.RequestPagingItems != null)
            {
                var e = new RequestPagingItemsEventArgs(new PagingInfo(pageIndex, this.RowsPerPage));
                this.RequestPagingItems(this,e);

                this.PagingItems = e.Result;
            }
        }

        public void RaiseRequestPagingItems(int pageIndex=0)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            OnRequestPagingItems(pageIndex);
        }

        public int RowsPerPage
        {
            get { return (int)GetValue(RowsPerPageProperty); }
            set { SetValue(RowsPerPageProperty, value); }
        }

        private int PageIndex
        { get { return this.PagingItems == null ? 0 : this.PagingItems.PageIndex; } }

        public int PageCount { get { return this.PagingItems == null ? 0 : this.PagingItems.PageCount; } }
    }

    public class RequestPagingItemsEventArgs:EventArgs
    {
        public RequestPagingItemsEventArgs(PagingInfo pagingInfo)
        {
            this.PagingInfo = pagingInfo;
        }
        public PagingInfo PagingInfo { get; private set; }

        public PagingItems Result
        { get; set; }
    }
}
