using System;

namespace FCSPlayout.PlayEngine
{
    public class PlayerItemEventArgs : EventArgs
    {
        public PlayerItemEventArgs(IPlayerItem playItem)
        {
            this.PlayerItem = playItem;
        }

        public IPlayerItem PlayerItem
        {
            get;
            private set;
        }
    }
}
