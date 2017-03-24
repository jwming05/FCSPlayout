using Prism.Interactivity.InteractionRequest;

namespace FCSPlayout.MediaFileImporter
{
    public class PlayableItemPreviewNotification:Notification
    {
        public IPlayableItem PlayableItem { get; set; }
    }
}