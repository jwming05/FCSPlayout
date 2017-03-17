﻿using FCSPlayout.CG;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// CGImageItemListView.xaml 的交互逻辑
    /// </summary>
    public partial class CGImageItemListView : UserControl
    {
        

        //private static readonly DependencyPropertyKey CGItemsListKey =
        //    DependencyProperty.RegisterReadOnly("CGItemsList", typeof(ObservableCollection<ViewModels.BindableCGImageItem>), typeof(CGImageItemListView), 
        //        new FrameworkPropertyMetadata(null));

        //public static readonly DependencyProperty CGItemsListProperty =CGItemsListKey.DependencyProperty;

        
        public static readonly DependencyProperty CGItemsProperty =
            DependencyProperty.Register("CGItems", typeof(CG.CGItemCollection), typeof(CGImageItemListView), 
                new FrameworkPropertyMetadata(null, OnCGItemsPropertyChanged));
        private CGImageItemListViewModel _viewModel;

        private static void OnCGItemsPropertyChanged(DependencyObject dpObj,DependencyPropertyChangedEventArgs e)
        {
            ((CGImageItemListView)dpObj).OnCGItemsChanged((CG.CGItemCollection)e.OldValue,(CG.CGItemCollection)e.NewValue);
        }

        public CGImageItemListView()
        {
            InitializeComponent();
            _viewModel = new CGImageItemListViewModel();
            this.DataContext = _viewModel;
            //this.SetValue(CGItemsListKey, new ObservableCollection<ViewModels.BindableCGImageItem>());
        }

        //public ObservableCollection<ViewModels.BindableCGImageItem> CGItemsList
        //{
        //    get { return (ObservableCollection<ViewModels.BindableCGImageItem>)GetValue(CGItemsListProperty); }
        //}

        public CG.CGItemCollection CGItems
        {
            get { return (CG.CGItemCollection)GetValue(CGItemsProperty); }
            set { SetValue(CGItemsProperty, value); }
        }

        public CG.CGItemCollection NewCGItems
        {
            get { return _viewModel.CGItems; }
        }

        private void OnCGItemsChanged(CGItemCollection oldValue, CGItemCollection newValue)
        {
            _viewModel.CGItems = this.CGItems;
        }
    }
}
