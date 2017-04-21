using FCSPlayout.Domain;

namespace FCSPlayout.AppInfrastructure
{
    public interface IMediaItemSelector
    {
        MediaItem? SelectedMediaItem { get; set; }
    }
}