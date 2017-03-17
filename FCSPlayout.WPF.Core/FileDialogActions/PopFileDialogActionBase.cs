using Microsoft.Win32;
using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Interactivity;

namespace FCSPlayout.WPF.Core
{
    public abstract class PopFileDialogActionBase<T> : TriggerAction<FrameworkElement>
        where T : FileDialog, new()
    {
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(PopFileDialogActionBase<T>),
                new PropertyMetadata(string.Empty, OnFilterPropertyChanged));
        public static readonly DependencyProperty FilterIndexProperty =
            DependencyProperty.Register("FilterIndex", typeof(int), typeof(PopFileDialogActionBase<T>),
                new PropertyMetadata(-1, OnFilterIndexPropertyChanged));


        private static void OnFilterPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PopFileDialogActionBase<T>)dpObj).OnFilterChanged((string)e.OldValue, (string)e.NewValue);
        }

        private static void OnFilterIndexPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PopFileDialogActionBase<T>)dpObj).OnFilterIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual void OnFilterChanged(string oldValue, string newValue)
        {
            FileDialog.Filter = this.Filter;
        }

        protected virtual void OnFilterIndexChanged(int oldValue, int newValue)
        {
            FileDialog.FilterIndex = this.FilterIndex;
        }

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public int FilterIndex
        {
            get { return (int)GetValue(FilterIndexProperty); }
            set { SetValue(FilterIndexProperty, value); }
        }

        protected T FileDialog
        {
            get
            {
                if (_fileDialog == null)
                {
                    _fileDialog = new T();
                }
                return _fileDialog;
            }
            set
            {
                _fileDialog = value;
            }
        }

        private T _fileDialog;

        protected override void Invoke(object parameter)
        {
            var e = parameter as InteractionRequestedEventArgs;
            if (e != null)
            {
                FileDialogConfirmation confirmation = e.Context as FileDialogConfirmation;

                if (confirmation != null)
                {
                    ConfigDialog(confirmation);
                }

                if (FileDialog.ShowDialog() == true)
                {
                    if (confirmation != null)
                    {
                        OnOK(confirmation);
                    }
                }

                if (e.Callback != null)
                {
                    e.Callback();
                }
            }
        }

        protected virtual void ConfigDialog(FileDialogConfirmation confirmation)
        {
            if (!string.IsNullOrEmpty(confirmation.Filter))
            {
                FileDialog.Filter = confirmation.Filter;
            }

            if (confirmation.FilterIndex >= 0)
            {
                FileDialog.FilterIndex = confirmation.FilterIndex;
            }
        }

        protected virtual void OnOK(FileDialogConfirmation confirmation)
        {
            confirmation.Confirmed = true;
            confirmation.FileName = FileDialog.FileName;
            confirmation.FileNames = FileDialog.FileNames;
        }
    }
}
