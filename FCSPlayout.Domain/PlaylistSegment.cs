using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public struct PlaylistSegment
    {

        public static PlaylistSegment Invalid = new PlaylistSegment { IsValid = false };
        private IPlayItem _head;

        public static PlaylistSegment CreateValid(int headIndex, IPlayItem head)
        {
            return new PlaylistSegment
            {
                IsValid = true,
                HeadIndex = headIndex,
                Head = head
            };
        }

        public int HeadIndex { get; internal set; }
        public IPlayItem Head
        {
            get { return _head; }
            internal set
            {
                _head = value;
                if (_head != null)
                {
                    this.StartTime = _head.StartTime;
                }
            }
        }

        public bool IsValid { get; private set; }

        public DateTime StartTime { get; internal set; }
    }
}
