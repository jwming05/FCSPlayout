using System;
using System.Runtime.Serialization;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class InvalidPlayRangeException : Exception
    {
        public PlayRange PlayRange { get; private set; }


        public InvalidPlayRangeException(PlayRange playRange)
        {
            this.PlayRange = playRange;
        }
    }
}