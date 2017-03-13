using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlaylistEditor : IPlaylistEditor
    {
        private IPlaylist _playlist;
        private PlaylistBuilder _builder;

        private void AddAutoAfter(IPlayItem prevItem,AutoPlayItem newItem)
        {
            if (!_playlist.Contains(prevItem))
            {
                throw new ArgumentException();
            }

            // 不能在定时插播后插入一条顺播。
            if (prevItem.PlaybillItem.Category == PlaybillItemCategory.TimingBreak)
            {
                throw new ArgumentException();
            }

            if (prevItem.PlaybillItem.Category == PlaybillItemCategory.Timing)
            {

            }
            else // (prevItem.PlaybillItem.Category == PlaybillItemCategory.Auto)
            {

            }
        }

        private void InsertTiming(TimingPlaybillItem playbillItem)
        {
            // 验证时间范围（包含开始时间验证）。
            _playlist.ValidateTimeRange(playbillItem.StartTime.Value, playbillItem.PlaySource.GetDuration());

            PlaylistSegment segment = _playlist.FindLastSegment(i => i.StartTime < playbillItem.StartTime.Value);
            DateTime startTime, stopTime;
            int beginIndex, endIndex;

            if (segment.IsValid)
            {
                if (segment.Head != null)
                {
                    startTime = segment.Head.GetStopTime();
                }
                else
                {
                    startTime = segment.StartTime;
                }
                

                beginIndex = segment.HeadIndex + 1; // 可能等于_playlist.Count

                var nextSegment = _playlist.GetNextSegment(segment);

                // NOTE: endIndex可能小于beginIndex
                if (nextSegment.IsValid)
                {
                    Debug.Assert(nextSegment.Head != null);

                    stopTime = nextSegment.StartTime;
                    endIndex = nextSegment.HeadIndex - 1; // 可能等于beginIndex-1
                }
                else
                {
                    stopTime = DateTime.MaxValue;
                    endIndex = _playlist.Count - 1;
                }
            }
            else
            {
                // case 1: _playlist为空。 beginIndex=0,endIndex=-1
                // case 2: _playlist不为空（至少包含一个片断）。
                //     case 2.1: 第一个片断有定时播。 beginIndex=0,endIndex=-1
                //     case 2.2: 第一个片断没有定时播。
                //         case 2.2.1: 只有一个片断。 beginIndex=0,endIndex=_playlist.Count - 1
                //         case 2.2.2: 有二个或二个以上片断。 beginIndex=0,endIndex=X


                startTime = playbillItem.StartTime.Value;
                beginIndex = 0;

                var nextSegment = _playlist.FindFirstSegment(i => i.Head != null);

                if (nextSegment.IsValid)
                {
                    // case 2.1
                    // case 2.2.2
                    stopTime = nextSegment.StartTime;
                    endIndex = nextSegment.HeadIndex - 1;
                }
                else
                {
                    // case 1
                    // case 2.2.1
                    stopTime = DateTime.MaxValue;
                    endIndex = _playlist.Count - 1;
                }
            }

            IList<IPlayItem> playItems = _playlist.GetPlayItems(beginIndex, endIndex);

            PlaylistBuildData data = new PlaylistBuildData();
            data.StartTime = startTime;
            data.StopTime = stopTime;
            for (int i = 0; i < playItems.Count; i++)
            {
                var item = playItems[i];
                if (item.PlaybillItem.Category == PlaybillItemCategory.Auto)
                {
                    data.AddAuto((AutoPlayItem)item);
                }
                else
                {
                    data.InsertTiming((TimingPlaybillItem)item);
                }
            }

            data.InsertTiming(playbillItem);

            IList<IPlayItem> newPlayItems = _builder.Build(data);

            _playlist.Update(beginIndex, endIndex - beginIndex + 1, newPlayItems);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
