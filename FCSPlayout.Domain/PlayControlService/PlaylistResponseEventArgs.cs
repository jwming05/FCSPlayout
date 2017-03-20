using System;

namespace FCSPlayout.Domain
{
    public class PlaylistResponseEventArgs : EventArgs
    {
        public PlaylistResponseEventArgs(PlaylistResponseMessage message)
        {
            this.ResponseMessage = message;
        }

        public PlaylistResponseMessage ResponseMessage { get; set; }
    }
}
