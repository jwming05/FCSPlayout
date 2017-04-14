using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Controls;
using System;

namespace FCSPlayout.WPF.Core
{
    public class ViewBase : UserControl
    {
        public static readonly DependencyProperty SaveFileInteractionRequestProperty =
            DependencyProperty.Register("SaveFileInteractionRequest", typeof(InteractionRequest<SaveFileDialogConfirmation>),
                typeof(ViewBase), new FrameworkPropertyMetadata(null, OnSaveFileInteractionRequestPropertyChanged));

        public static readonly DependencyProperty OpenFileInteractionRequestProperty =
            DependencyProperty.Register("OpenFileInteractionRequest", typeof(InteractionRequest<OpenFileDialogConfirmation>), typeof(ViewBase),
                new FrameworkPropertyMetadata(null, OnOpenFileInteractionRequestPropertyChanged));

        public static readonly DependencyProperty EditMediaItemInteractionRequestProperty =
            DependencyProperty.Register("EditMediaItemInteractionRequest", typeof(InteractionRequest<EditMediaItemConfirmation>),
                typeof(ViewBase), new FrameworkPropertyMetadata(null, OnEditMediaItemInteractionRequestPropertyChanged));

        //public static readonly DependencyProperty EditDurationInteractionRequestProperty =
        //    DependencyProperty.Register("EditDurationInteractionRequest", typeof(InteractionRequest<EditDurationConfirmation>),
        //        typeof(ViewBase), new FrameworkPropertyMetadata(null, OnEditDurationInteractionRequestPropertyChanged));

        private static void OnOpenFileInteractionRequestPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((ViewBase)dpObj).OnOpenFileInteractionRequestChanged((InteractionRequest<OpenFileDialogConfirmation>)e.OldValue, (InteractionRequest<OpenFileDialogConfirmation>)e.NewValue);
        }

        private static void OnSaveFileInteractionRequestPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((ViewBase)dpObj).OnSaveFileInteractionRequestChanged((InteractionRequest<SaveFileDialogConfirmation>)e.OldValue, (InteractionRequest<SaveFileDialogConfirmation>)e.NewValue);
        }

        private static void OnEditMediaItemInteractionRequestPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((ViewBase)dpObj).OnEditMediaItemInteractionRequestChanged((InteractionRequest<EditMediaItemConfirmation>)e.OldValue, (InteractionRequest<EditMediaItemConfirmation>)e.NewValue);
        }

        //private static void OnEditDurationInteractionRequestPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        //{
        //    ((ViewBase)dpObj).OnEditDurationInteractionRequestChanged((InteractionRequest<EditDurationConfirmation>)e.OldValue, (InteractionRequest<EditDurationConfirmation>)e.NewValue);
        //}

        public InteractionRequest<EditMediaItemConfirmation> EditMediaItemInteractionRequest
        {
            get { return (InteractionRequest<EditMediaItemConfirmation>)GetValue(EditMediaItemInteractionRequestProperty); }
            set { SetValue(EditMediaItemInteractionRequestProperty, value); }
        }

        //public InteractionRequest<EditDurationConfirmation> EditDurationInteractionRequest
        //{
        //    get { return (InteractionRequest<EditDurationConfirmation>)GetValue(EditDurationInteractionRequestProperty); }
        //    set { SetValue(EditDurationInteractionRequestProperty, value); }
        //}

        public InteractionRequest<SaveFileDialogConfirmation> SaveFileInteractionRequest
        {
            get { return (InteractionRequest<SaveFileDialogConfirmation>)GetValue(SaveFileInteractionRequestProperty); }
            set { SetValue(SaveFileInteractionRequestProperty, value); }
        }

        public InteractionRequest<OpenFileDialogConfirmation> OpenFileInteractionRequest
        {
            get { return (InteractionRequest<OpenFileDialogConfirmation>)GetValue(OpenFileInteractionRequestProperty); }
            set { SetValue(OpenFileInteractionRequestProperty, value); }
        }

        protected virtual void OnSaveFileInteractionRequestChanged(InteractionRequest<SaveFileDialogConfirmation> oldValue, InteractionRequest<SaveFileDialogConfirmation> newValue)
        {
            if (this.ViewModel != null)
            {
                ViewModel.SaveFileInteractionRequest = this.SaveFileInteractionRequest;
            }
        }

        private void OnEditMediaItemInteractionRequestChanged(InteractionRequest<EditMediaItemConfirmation> oldValue, InteractionRequest<EditMediaItemConfirmation> newValue)
        {
            if (this.ViewModel != null)
            {
                ViewModel.EditMediaItemInteractionRequest = this.EditMediaItemInteractionRequest;
            }
        }

        //private void OnEditDurationInteractionRequestChanged(InteractionRequest<EditDurationConfirmation> oldValue, InteractionRequest<EditDurationConfirmation> newValue)
        //{
        //    if (this.ViewModel != null)
        //    {
        //        ViewModel.EditDurationInteractionRequest = this.EditDurationInteractionRequest;
        //    }
        //}

        protected virtual void OnOpenFileInteractionRequestChanged(InteractionRequest<OpenFileDialogConfirmation> oldValue, InteractionRequest<OpenFileDialogConfirmation> newValue)
        {
            if (this.ViewModel != null)
            {
                ViewModel.OpenFileInteractionRequest = this.OpenFileInteractionRequest;
            }
        }

        protected virtual ViewModelBase ViewModel
        {
            get; set;
        }
    }
}
