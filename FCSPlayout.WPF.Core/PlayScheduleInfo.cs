using FCSPlayout.Domain;
using System;

namespace FCSPlayout.WPF.Core
{
    public class PlayScheduleInfo : IEquatable<PlayScheduleInfo>
    {
        private static readonly PlayScheduleInfo _ordered = new PlayScheduleInfo();

        public static PlayScheduleInfo Timing(DateTime startTime)
        {
            return new PlayScheduleInfo(startTime, false);
        }

        public static PlayScheduleInfo TimingBreak(DateTime startTime)
        {
            return new PlayScheduleInfo(startTime, true);
        }

        public static PlayScheduleInfo Ordered()
        {
            return _ordered;
            //return new PlayMode();
        }

        public bool Equals(PlayScheduleInfo other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.Mode == other.Mode && this.StartTime == other.StartTime;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PlayScheduleInfo);
        }

        public override int GetHashCode()
        {
            if (this.StartTime == null)
            {
                return this.Mode.GetHashCode();
            }
            else
            {
                return this.Mode.GetHashCode() ^ this.StartTime.GetHashCode();
            }
        }

        public static bool operator ==(PlayScheduleInfo left, PlayScheduleInfo right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }
            else
            {
                return object.ReferenceEquals(left, null) ? false : left.Equals(right);
            }
        }

        public static bool operator !=(PlayScheduleInfo left, PlayScheduleInfo right)
        {
            return !(left == right);
        }

        private PlayScheduleInfo(DateTime startTime, bool isBreak)
        {
            this.StartTime = startTime;
            this.Mode = isBreak ? PlayScheduleMode.TimingBreak : PlayScheduleMode.Timing;
        }

        private PlayScheduleInfo()
        {
            this.Mode = PlayScheduleMode.Auto;
        }

        public PlayScheduleMode Mode { get; private set; }

        public DateTime? StartTime
        {
            get; set;
        }

        //#region
        //public const string XElementTag = "play-mode";

        //public static XElement ToXElement(PlayMode playMode)
        //{
        //    XElement element = new XElement("play-mode");
        //    element.Add(new XAttribute("category", playMode.Category));
        //    if (playMode.StartTime != null)
        //    {
        //        element.Add(new XAttribute("start-time",
        //            XElementSerializationUtils.DateTimeToString(playMode.StartTime.Value)));
        //    }

        //    return element;
        //}

        //public static PlayMode FromXElement(XElement element)
        //{
        //    if (element.Name != "play-mode")
        //    {
        //        throw new ArgumentException();
        //    }

        //    var category = (PlayModeCategory)Enum.Parse(typeof(PlayModeCategory), element.Attribute("category").Value);
        //    DateTime startTime;
        //    switch (category)
        //    {
        //        case PlayModeCategory.Ordered:
        //            return PlayMode.Ordered();
        //        case PlayModeCategory.Timing:
        //        case PlayModeCategory.TimingBreak:
        //            startTime = XElementSerializationUtils.DateTimeFromString(element.Attribute("start-time").Value);
        //            return category == PlayModeCategory.Timing ? PlayMode.Timing(startTime) : PlayMode.TimingBreak(startTime);
        //        default:
        //            break;
        //    }

        //    throw new System.ComponentModel.InvalidEnumArgumentException("PlayModeCategory",
        //        (int)category, typeof(PlayModeCategory));
        //}
        //#endregion
    }
}
