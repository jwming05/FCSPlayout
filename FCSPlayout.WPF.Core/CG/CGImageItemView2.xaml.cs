using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FCSPlayout.CG;
using Prism.Interactivity.InteractionRequest;
using FCSPlayout.WPF.Core;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// CGImageItemView.xaml 的交互逻辑
    /// </summary>
    public partial class CGImageItemView2 : UserControl,Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        //private CGItemCollection _cgItems=new CGItemCollection();
        private EditCGItemsConfirmation _notification;

        public static readonly DependencyProperty CGItemsProperty =
            DependencyProperty.Register("CGItems", typeof(CG.CGItemCollection), typeof(CGImageItemView2),
                new FrameworkPropertyMetadata(null));

        public CGImageItemView2()
        {
            InitializeComponent();
        }

        public Action FinishInteraction
        {
            get;set;
        }

        public INotification Notification
        {
            get { return _notification; }
            set
            {
                _notification = value as EditCGItemsConfirmation;
                if (_notification != null)
                {
                    this.CGItems = _notification.Items;
                    //_cgItems.Reset(_notification.Items);
                }
            }
        }
        //public CGItemCollection CGItems
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(this.filePicker.SelectedFile) && System.IO.File.Exists(this.filePicker.SelectedFile))
        //        {
        //            _cgItems.Add(new MLCGImageItem { File = this.filePicker.SelectedFile, X = this.xUpDown.Value ?? 0, Y = this.yUpDown.Value ?? 0, IsRelative = false });
        //        }
        //        return _cgItems;
        //    }
        //}

        public CG.CGItemCollection CGItems
        {
            get { return (CG.CGItemCollection)GetValue(CGItemsProperty); }
            set { SetValue(CGItemsProperty, value); }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var confirm=(FCSPlayout.WPF.Core.EditCGItemsConfirmation)this.Notification;
            confirm.Confirmed = true;
            CGItemCollection newItems = new CGItemCollection();
            newItems.AddRange(this.imageItemListView.NewCGItems);
            newItems.AddRange(this.textItemListView.NewCGItems);
            newItems.AddRange(this.tickerItemListView.NewCGItems);
            confirm.Items = newItems; // this.imageItemListView.NewCGItems; // this.CGItems;
            this.FinishInteraction();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var confirm = (FCSPlayout.WPF.Core.EditCGItemsConfirmation)this.Notification;
            confirm.Confirmed = false;
            this.FinishInteraction();
        }
    }
}
