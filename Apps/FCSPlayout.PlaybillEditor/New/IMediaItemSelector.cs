using FCSPlayout.Domain;

namespace FCSPlayout.PlaybillEditor
{
    public interface IMediaItemSelector
    {
        MediaItem? SelectedMediaItem { get; set; }
    }
}