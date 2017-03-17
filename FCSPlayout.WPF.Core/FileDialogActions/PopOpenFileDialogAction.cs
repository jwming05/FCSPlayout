using Microsoft.Win32;
using System.Windows;

namespace FCSPlayout.WPF.Core
{
    public class PopOpenFileDialogAction : PopFileDialogActionBase<OpenFileDialog>
    {
        public static readonly DependencyProperty MultiselectProperty =
            DependencyProperty.Register("Multiselect", typeof(bool), typeof(PopOpenFileDialogAction),
                new PropertyMetadata(false, OnMultiselectPropertyChanged));

        private static void OnMultiselectPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PopOpenFileDialogAction)dpObj).OnMultiselectChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private void OnMultiselectChanged(bool oldValue, bool newValue)
        {
            base.FileDialog.Multiselect = this.Multiselect;
        }

        public bool Multiselect
        {
            get { return (bool)GetValue(MultiselectProperty); }
            set { SetValue(MultiselectProperty, value); }
        }

        protected override void ConfigDialog(FileDialogConfirmation confirmation)
        {
            var c = (OpenFileDialogConfirmation)confirmation;
            if (c.Multiselect != null)
            {
                this.FileDialog.Multiselect = c.Multiselect.Value;
            }
            base.ConfigDialog(confirmation);
        }
    }
}
