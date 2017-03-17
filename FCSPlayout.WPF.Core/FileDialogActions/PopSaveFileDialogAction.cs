using Microsoft.Win32;
using System.Windows;

namespace FCSPlayout.WPF.Core
{
    public class PopSaveFileDialogAction : PopFileDialogActionBase<SaveFileDialog>
    {
        public static readonly DependencyProperty OverwritePromptProperty =
            DependencyProperty.Register("OverwritePrompt", typeof(bool), typeof(PopSaveFileDialogAction),
                new PropertyMetadata(false, OnOverwritePromptPropertyChanged));

        private static void OnOverwritePromptPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PopSaveFileDialogAction)dpObj).OnOverwritePromptChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private void OnOverwritePromptChanged(bool oldValue, bool newValue)
        {
            base.FileDialog.OverwritePrompt = this.OverwritePrompt;
        }

        public bool OverwritePrompt
        {
            get { return (bool)GetValue(OverwritePromptProperty); }
            set { SetValue(OverwritePromptProperty, value); }
        }

        protected override void ConfigDialog(FileDialogConfirmation confirmation)
        {
            SaveFileDialogConfirmation c = (SaveFileDialogConfirmation)confirmation;
            if (c.OverwritePrompt != null)
            {
                this.FileDialog.OverwritePrompt = c.OverwritePrompt.Value;
            }

            base.ConfigDialog(confirmation);
        }
    }
}
