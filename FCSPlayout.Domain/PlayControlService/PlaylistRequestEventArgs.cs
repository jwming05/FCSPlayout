using System;

namespace FCSPlayout.Domain
{
    public class PlaylistRequestEventArgs : EventArgs
    {
        public PlaylistRequestEventArgs(PlaylistRequestMessage message)
        {
            this.RequestMessage = message;
        }

        public PlaylistRequestMessage RequestMessage { get; set; }
    }
}
