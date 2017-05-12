using System;

namespace FCSPlayout.PlayEngine
{
    public interface IPlayPreview
    {
        IPlayerItem CurrentPlayItem { get; set; }
        TimeSpan CurrentPlayItemPosition { get; set; }
        double PlaylistPosition { get; set; }

        void SetPreviewUri(Uri uri);

        MPLATFORMLib.IMObject MObject { get; set; }
    }
}