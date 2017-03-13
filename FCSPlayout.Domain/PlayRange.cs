using System;

namespace FCSPlayout.Domain
{
    public struct PlayRange:IEquatable<PlayRange>
    {
        #region Static Members
        internal static void Split(PlayRange range, TimeSpan duration, out PlayRange first, out PlayRange second)
        {
            first = new PlayRange(range.StartPosition, duration);
            second = new PlayRange(first.StopPosition, range.Duration - duration);
        }

        internal static bool CanMerge(PlayRange range1, PlayRange range2)
        {
            return range1.StopPosition == range2.StartPosition || range2.StopPosition == range1.StartPosition;
        }

        internal static PlayRange Merge(PlayRange range1, PlayRange range2)
        {
            if (CanMerge(range1, range2)) throw new InvalidOperationException();

            var startPos = range1.StartPosition < range2.StartPosition ? range1.StartPosition : range2.StartPosition;

            return new PlayRange(startPos, range1.Duration + range2.Duration);
        }
        #endregion Static Members


        public PlayRange(TimeSpan duration)
            :this(TimeSpan.Zero,duration)
        {
        }

        public PlayRange(TimeSpan startPosition,TimeSpan duration)
        {
            if (startPosition < TimeSpan.Zero)
            {
                throw new ArgumentException(string.Format("值{0}无效，不能小于TimeSpan.Zero。",startPosition), 
                    "startPosition");
            }

            if (duration < TimeSpan.Zero)
            {
                throw new ArgumentException(string.Format("值{0}无效，不能小于TimeSpan.Zero。", duration), 
                    "duration");
            }

            this.StartPosition = startPosition;
            this.Duration = duration;
        }

        public TimeSpan Duration { get; private set; }
        public TimeSpan StartPosition { get; private set; }

        public TimeSpan StopPosition
        {
            get { return StartPosition + Duration; }
        }

        

        #region
        public bool Equals(PlayRange other)
        {
            return this.StartPosition == other.StartPosition && this.Duration == other.Duration;
        }

        public override bool Equals(object obj)
        {
            return this.Equals((PlayRange)obj);
        }

        public override int GetHashCode()
        {
            return this.StartPosition.GetHashCode() ^ this.Duration.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]",this.StartPosition,this.StopPosition);
        }

        public static bool operator==(PlayRange left, PlayRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayRange left, PlayRange right)
        {
            return !(left == right);
        }
        #endregion
    }
}