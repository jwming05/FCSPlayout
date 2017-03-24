using System;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class PlaylistEditException:Exception
    {
        public PlaylistEditException()
        {
        }

        public PlaylistEditException(string message)
            :base(message)
        {
        }
    }
}
