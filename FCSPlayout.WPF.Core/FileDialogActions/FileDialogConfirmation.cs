using Prism.Interactivity.InteractionRequest;

namespace FCSPlayout.WPF.Core
{
    public class FileDialogConfirmation : Confirmation
    {
        private int _filterIndex;

        public string Filter { get; set; }
        public int FilterIndex
        {
            get { return _filterIndex - 1; }
            set
            {
                _filterIndex = value + 1;
            }
        }

        public string FileName { get; internal set; }
        public string[] FileNames { get; internal set; }
    }

    public class OpenFileDialogConfirmation : FileDialogConfirmation
    {
        public bool? Multiselect { get; set; }
    }

    public class SaveFileDialogConfirmation : FileDialogConfirmation
    {
        public bool? OverwritePrompt { get; set; }
    }
}
