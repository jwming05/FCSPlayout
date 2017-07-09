using System;
using System.Collections.Generic;

namespace FCSPlayout.Domain
{
    internal partial class PlaylistSegmentCollection
    {
        private IPlaylistBuildOption _buildOption;
        public PlaylistSegmentCollection(IList<IPlayItem> playItems, IPlaylistBuildOption buildOption)
        {
            _buildOption = buildOption;
            List<IPlayItem> items = new List<IPlayItem>();

            for(int i = 0; i < playItems.Count; i++)
            {
                var item = playItems[i];
                if (item.ScheduleMode == PlayScheduleMode.Timing)
                {
                    if (items.Count > 0)
                    {
                        PlaylistSegment segment = new PlaylistSegment(items);
                        AddLast(segment);
                        items.Clear();
                    }
                }

                items.Add(item);
            }

            if (items.Count > 0)
            {
                PlaylistSegment segment = new PlaylistSegment(items);
                AddLast(segment);
                items.Clear();
            }

            var seg = this.FirstSegment;
            while (seg != null)
            {
                seg.IsDirty = false;
                seg = seg.Next;
            }
        }

        public bool IsEmpty
        {
            get { return this.FirstSegment == null; }
        }

        public PlaylistSegment FirstSegment
        {
            get; private set;
        }

        public PlaylistSegment LastSegment
        {
            get;private set;
        }

        public PlaylistSegment CreateSegment(IPlayItem playItem)
        {
            return new PlaylistSegment(playItem) { IsDirty = true };
        }

        public PlaylistSegment CreateSegment(DateTime startTime)
        {
            return new PlaylistSegment(startTime) { IsDirty = true };
        }

        public PlaylistSegment CreateSegment(DateTime startTime, IPlayItem playItem)
        {
            return new PlaylistSegment(startTime, playItem) { IsDirty = true };
        }

        internal void AddLast(PlaylistSegment segment)
        {
            if (IsEmpty)
            {
                this.FirstSegment = this.LastSegment = segment;
            }
            else
            {
                this.LastSegment.Next = segment;
                this.LastSegment = segment;
            }
        }

        internal void AddFirst(PlaylistSegment segment)
        {
            if (IsEmpty)
            {
                this.FirstSegment = this.LastSegment = segment;
            }
            else
            {
                segment.Next = this.FirstSegment;
                this.FirstSegment = segment;
            }
        }

        internal IEnumerable<IPlayItem> Build(DateTime maxStopTime)
        {
            var builders = CreateBuilders(maxStopTime);
            foreach(var builder in builders)
            {
                foreach(var item in builder.Build())
                {
                    yield return item;
                }
            }
        }

        public PlaylistSegment FindLastSegment(Predicate<PlaylistSegment> predicate)
        {
            var segment = this.LastSegment;
            while (segment != null)
            {
                if (predicate(segment))
                {
                    return segment;
                }

                segment = segment.Previous;
            }
            return null;
        }

        private IEnumerable<IPlaylistSegmentBuilder> CreateBuilders(DateTime maxStopTime)
        {
            List<IPlaylistSegmentBuilder> builders = new List<IPlaylistSegmentBuilder>();
            var segment = this.FirstSegment;
            while (segment != null)
            {
                if(segment.Next!=null && segment.ShouldMerge())
                {
                    // 定时播消失（删除，改为顺播或定时插播）

                    segment.MergeFrom(segment.Next);
                    builders.Add(segment.CreateBuilder(maxStopTime));

                    segment = segment.Next.Next;
                }
                else
                {
                    builders.Add(segment.CreateBuilder(maxStopTime));
                    segment = segment.Next;
                }
                
            }
            return builders;
        }

        internal void Remove(PlaylistSegment segment)
        {
            var prevSegment = segment.Previous;
            if (prevSegment != null)
            {
                prevSegment.Next = segment.Next;
                if (prevSegment.Next == null)
                {
                    this.LastSegment = prevSegment;
                }
            }
            else
            {
                this.FirstSegment = segment.Next;
                if (this.FirstSegment == null)
                {
                    this.LastSegment = null;
                }
                else
                {
                    this.FirstSegment.Previous = null;
                }
            }            
        }

        internal void InsertSegment(PlaylistSegment prevSegment, PlaylistSegment newSegment)
        {
            var oldNext = prevSegment.Next;
            prevSegment.Next = newSegment;
            newSegment.Next = oldNext;

            if (this.LastSegment == prevSegment)
            {
                this.LastSegment = newSegment;
            }
        }
    }

    internal partial class PlaylistSegmentCollection
    {        
    }
}