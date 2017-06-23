using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public partial class NewPlaylistEditor
    {
        /// <summary>
        /// 在列表末尾添加一个顺播。
        /// </summary>
        /// <param name="playItem"></param>
        public void AddAuto(IPlayItem playItem)
        {
            Debug.Assert(playItem.ScheduleMode == PlayScheduleMode.Auto);

            var prevTuple = FindLastTiming(i => true);
            DateTime startTime, stopTime = DateTime.MaxValue;
            int beginIndex, endIndex = _playlist.Count - 1;

            if (prevTuple.Item2 == -1)
            {
                if (_playlist.Count > 0)
                {
                    startTime = _playlist[0].StartTime;
                    beginIndex = 0;
                }
                else
                {
                    var playlistStartTime = GetPlaylistStartTime();
                    if (playlistStartTime == null)
                    {
                        throw new InvalidOperationException("第一个播放项必须是定时播。");
                    }

                    startTime = playlistStartTime.Value;
                    beginIndex = 0;
                }
            }
            else
            {
                startTime = prevTuple.Item1.CalculatedStopTime;
                beginIndex = prevTuple.Item2 + 1;
            }

            Rebuild(startTime, stopTime, beginIndex, endIndex, (items) => items.Add(new ScheduleItem(playItem)));
        }

        /// <summary>
        /// 删除指定的播放项。
        /// </summary>
        /// <param name="playItem"></param>
        public void Delete(IPlayItem playItem)
        {
            if (!_playlist.Any(i => i.PlayItem == playItem))
            {
                throw new InvalidOperationException();
            }
            //if (_playlist.IsLocked(playItem)) throw new PlayItemLockedException();

            var prevTuple = this.FindLastTiming(i => i.StartTime < playItem.StartTime);

            DateTime startTime, stopTime;
            int beginIndex, endIndex;

            if (prevTuple.Item2 == -1)
            {
                startTime = _playlist[0].StartTime;
                beginIndex = 0;
            }
            else
            {
                startTime = _playlist[prevTuple.Item2].CalculatedStopTime;
                beginIndex = prevTuple.Item2 + 1;
            }

            var nextTuple = this.FindFirstTiming(beginIndex, i => i.StartTime > playItem.StartTime);

            if (nextTuple.Item2 == -1)
            {
                stopTime = DateTime.MaxValue;
                endIndex = _playlist.Count - 1;
            }
            else
            {
                stopTime = _playlist[nextTuple.Item2].StartTime;
                endIndex = nextTuple.Item2 - 1;
            }

            this.Rebuild(startTime, stopTime, beginIndex, endIndex, (items) =>
            {
                var idx=items.FindIndex(i => i.PlayItem == playItem);
                items.RemoveAt(idx);
            });
        }

        /// <summary>
        /// 添加一个定时播或定时插播。
        /// </summary>
        /// <param name="playItem"></param>
        public void AddTiming(IPlayItem playItem)
        {
            Debug.Assert(playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing || 
                playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak);

            // 验证时间范围（包含开始时间验证）。
            ValidateTimeRange(playItem.StartTime, playItem.CalculatedPlayDuration);

            if (playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing)
            {
                // 是否已经存在定时播。
                var temp = this.FindLastTiming(i => true);
                if (temp.Item2 != -1)
                {
                    // 如果已经存在定时播，则开始时间必须在最后一个定时播结束时间之后。
                    if (playItem.StartTime < temp.Item1.CalculatedStopTime)
                    {
                        throw new ArgumentException();
                    }
                }
            }
            else if (playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak)
            {
                if (_playlist.Count == 0)
                {
                    //当列表为空时不能添加定时插播。
                    throw new InvalidOperationException("第一个播放项必须是定时播。");
                }

                if (playItem.StartTime < _playlist[0].StartTime)
                {
                    //新加的定时插播开始时间不能小于列表中第一项的开始时间。
                    throw new ArgumentException("定时插播的开始时间无效。");
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            var prevTuple = this.FindLastTiming(i => i.StartTime < playItem.StartTime);

            DateTime startTime, stopTime;
            int beginIndex, endIndex;

            if (prevTuple.Item2 == -1)
            {
                startTime = playItem.StartTime;
                if (_playlist.Count > 0 && _playlist[0].StartTime < startTime)
                {
                    startTime = _playlist[0].StartTime;
                }
                beginIndex = 0;
            }
            else
            {
                startTime = prevTuple.Item1.CalculatedStopTime;
                beginIndex = prevTuple.Item2 + 1;
            }

            var nextTuple = this.FindFirstTiming(beginIndex, i => i.StartTime > playItem.StartTime);

            if (nextTuple.Item2 == -1)
            {
                stopTime = DateTime.MaxValue;
                endIndex = _playlist.Count - 1;
            }
            else
            {
                stopTime = nextTuple.Item1.StartTime;
                endIndex = nextTuple.Item2 - 1;
            }

            this.Rebuild(startTime, stopTime, beginIndex, endIndex, (items) => items.Add(new ScheduleItem(playItem)));
        }

        /// <summary>
        /// 把指定的顺播上移一个位置。
        /// </summary>
        /// <param name="playItem"></param>
        public void MoveUp(IPlayItem playItem)
        {
            Debug.Assert(playItem.ScheduleMode == PlayScheduleMode.Auto);

            //if (_playlist.IsLocked(playItem)) throw new PlayItemLockedException();

            if (_playlist[0] == playItem) return;

            //var index = _playlist.FindFirstIndex(i => i == playItem);
            var index = _playlist.FindIndex(i => i.PlayItem == playItem);
            if (index == -1)
            {
                throw new ArgumentException();
            }

            if (index >= 2)
            {
                var newPrevItem = _playlist[index - 2];
                //Reorder(newPrevItem, playItem);
                Reorder(newPrevItem.PlayItem, playItem);
            }
            else
            {
                Debug.Assert(index == 1);

                var oldPrevItem = _playlist[0];
                if(oldPrevItem.ScheduleMode==PlayScheduleMode.Timing || oldPrevItem.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    return;
                }

                var playSource = new PlaySource(playItem.MediaSource/*.Clone()*/, playItem.PlayRange, playItem.CGItems);
                var newAutoItem = new AutoPlayItem(PlaybillItem.Auto(playSource));

                var nextTuple = this.FindFirstTiming(index+1, (i) => true);

                DateTime startTime = oldPrevItem.StartTime;
                int beginIndex = 0;
                DateTime stopTime;
                int endIndex;

                if (nextTuple.Item2 == -1)
                {
                    stopTime = DateTime.MaxValue;
                    endIndex = _playlist.Count - 1;
                }
                else
                {
                    stopTime = nextTuple.Item1.StartTime;
                    endIndex = nextTuple.Item2 - 1;
                }

                this.Rebuild(startTime, stopTime, beginIndex, endIndex, (items) => 
                {
                    items.RemoveAt(1);
                    items.Insert(0, new ScheduleItem(newAutoItem));
                });
            }
        }

        /// <summary>
        /// 把指定的顺播下移一个位置。
        /// </summary>
        /// <param name="playItem"></param>
        public void MoveDown(IPlayItem playItem)
        {
            Debug.Assert(playItem.ScheduleMode == PlayScheduleMode.Auto);

            //if (_playlist.IsLocked(playItem)) throw new PlayItemLockedException();

            if (_playlist[_playlist.Count - 1] == playItem) return;

            //var index = _playlist.FindFirstIndex(i => i == playItem);
            var index = _playlist.FindIndex(i => i.PlayItem == playItem);

            if (index == -1)
            {
                throw new ArgumentException();
            }


            var newPrevItem = _playlist[index + 1];
            //Reorder(newPrevItem, playItem);
            Reorder(newPrevItem.PlayItem, playItem);
        }

        /// <summary>
        /// 重新调整顺播位置。
        /// </summary>
        /// <param name="newPrevItem"></param>
        /// <param name="reorderItem"></param>
        public void Reorder(IPlayItem newPrevItem, IPlayItem reorderItem)
        {
            Debug.Assert(reorderItem.ScheduleMode == PlayScheduleMode.Auto);

            //var mediaSource = reorderItem.MediaSource; //.Clone();
            //var playSource = new PlaySource(mediaSource, reorderItem.PlayRange, reorderItem.CGItems);

            //var newAutoItem = new AutoPlayItem(PlaybillItem.Auto(playSource));

            this.Delete(reorderItem);
            //this.AddAuto(newPrevItem, newAutoItem);
            this.AddAuto(newPrevItem, reorderItem);
        }

        /// <summary>
        /// 在指定的播出项之后添加一条顺播。
        /// </summary>
        /// <param name="prevItem"></param>
        /// <param name="newItem"></param>
        public void AddAuto(IPlayItem prevItem, IPlayItem newItem)
        {
            Debug.Assert(newItem.ScheduleMode == PlayScheduleMode.Auto);

            //if (_playlist.IsLocked(prevItem)) throw new PlayItemLockedException();

            var segment = FindSegment(prevItem);

            DateTime startTime, stopTime;
            int beginIndex, endIndex;

            if (segment.Head == null)
            {
                startTime = segment.StartTime;
                beginIndex = segment.BeginIndex;
            }
            else
            {
                startTime = segment.Head.CalculatedStopTime;
                beginIndex = segment.BeginIndex + 1;
            }

            stopTime = segment.StopTime;
            endIndex = segment.EndIndex;

            this.Rebuild(startTime, stopTime, beginIndex, endIndex, (items) =>
            {
                // NOTE: FindIndex maybe -1 in case of prevItem is Timing
                var i = items.FindIndex(si=>si.PlayItem==prevItem);
                items.Insert(i + 1, new ScheduleItem(newItem));
            });
        }

        /// <summary>
        /// 变长替换媒体源。
        /// </summary>
        /// <param name="playItem"></param>
        /// <param name="mediaItem"></param>
        public void ChangeMediaSource(IPlayItem playItem, IMediaSource mediaSource, PlayRange playRange)
        {
            this.ChangeMediaSourceInternal(playItem, mediaSource, playRange);
        }

        /// <summary>
        /// 等长替换媒体源。
        /// </summary>
        /// <param name="playItem"></param>
        /// <param name="mediaSource"></param>
        public void ChangeMediaSource(IPlayItem playItem, IMediaSource mediaSource)
        {
            this.ChangeMediaSourceInternal(playItem, mediaSource, null);
        }

        /// <summary>
        /// 修改媒体源的入出点。
        /// </summary>
        /// <param name="playItem"></param>
        /// <param name="newRange"></param>
        public void ChangePlayRange(IPlayItem playItem, PlayRange newRange)
        {
            this.ChangeMediaSourceInternal(playItem, null, newRange);
        }

        /// <summary>
        /// 修改定时播或定时插播的开始时间。
        /// </summary>
        /// <param name="playItem"></param>
        /// <param name="startTime"></param>
        public void ChangeStartTime(IPlayItem playItem, DateTime startTime)
        {
            Debug.Assert(playItem.ScheduleMode == PlayScheduleMode.Timing || playItem.ScheduleMode == PlayScheduleMode.TimingBreak);
            this.ChangeToTimingScheduleInternal(playItem, null, startTime);
        }

        // 顺播改为定时播或定时插播。
        // 定时播或定时插播改为顺播。
        // 定时播改为定时插播。
        // 定时插播改为定时播。
        public void ChangeSchedule(IPlayItem playItem, PlayScheduleMode scheduleMode, DateTime? startTime)
        {
            Debug.Assert(scheduleMode != playItem.ScheduleMode);

            if (scheduleMode == PlayScheduleMode.Auto)
            {
                ChangeToAutoScheduleInternal(playItem);
            }
            else
            {
                ChangeToTimingScheduleInternal(playItem, scheduleMode, startTime);
            }
        }

        // case1: 当mediaSource为null，newPlayRange不为null时只改变入出点。
        // case2: 当mediaSource不为null，newPlayRange为null时等长替换媒体源。
        // case3: 当mediaSource不为null，newPlayRange不为null时变长替换媒体源。
        private void ChangeMediaSourceInternal(IPlayItem playItem, IMediaSource mediaSource, PlayRange? newPlayRange)
        {
            //if (_playlist.IsLocked(playItem)) throw new PlayItemLockedException();


            if (mediaSource == null && newPlayRange == null) return;
            if (mediaSource == null && newPlayRange.Value == playItem.PlayRange) return;

            if (newPlayRange != null && (playItem.ScheduleMode == PlayScheduleMode.Timing || playItem.ScheduleMode == PlayScheduleMode.TimingBreak))
            {
                //_playlist.ValidateTimeRange(playItem.StartTime, newPlayRange.Value.Duration, playItem);
                ValidateTimeRange(playItem.StartTime, newPlayRange.Value.Duration, playItem);
            }

            PlayRange playRange = newPlayRange ?? new PlayRange(TimeSpan.Zero, playItem.CalculatedPlayDuration/*playItem.PlayRange.Duration*/);

            if (mediaSource == null)
            {
                mediaSource = playItem.MediaSource; //.Clone();
            }

            var newPlaySource = new PlaySource(mediaSource, playRange, playItem.CGItems);
            
            IPlayItem newPlayItem = null;
            switch (playItem.ScheduleMode)
            {
                case PlayScheduleMode.Timing:
                    newPlayItem = (IPlayItem)PlaybillItem.Timing(newPlaySource, playItem.StartTime);
                    break;
                case PlayScheduleMode.TimingBreak:
                    newPlayItem = (IPlayItem)PlaybillItem.TimingBreak(newPlaySource, playItem.StartTime);
                    break;
                case PlayScheduleMode.Auto:
                    newPlayItem = new AutoPlayItem(PlaybillItem.Auto(newPlaySource));
                    break;
            }

            var segment = this.FindSegment(playItem);

            DateTime startTime = segment.StartTime;
            int beginIndex = segment.BeginIndex;

            DateTime stopTime = segment.StopTime;
            int endIndex = segment.EndIndex;

            Rebuild(startTime, stopTime, beginIndex, endIndex, (items) => 
            {
                var i = items.FindIndex(si => si.PlayItem == playItem);
                items[i] =new ScheduleItem(newPlayItem);
            });
        }

        // case1: 修改定时播或定时插播的开始时间。
        // case2: 定时播改为定时插播。 
        // case3: 定时插播改为定时播。
        // case4: 顺播改为定时播或定时插播。
        private void ChangeToTimingScheduleInternal(IPlayItem playItem, PlayScheduleMode? scheduleMode, DateTime? startTime)
        {
            var newScheduleMode = scheduleMode ?? playItem.ScheduleMode;

            Debug.Assert(newScheduleMode == PlayScheduleMode.Timing || newScheduleMode == PlayScheduleMode.TimingBreak);

            //if (_playlist.IsLocked(playItem)) throw new PlayItemLockedException();

            var newStartTime = startTime ?? playItem.StartTime;
            if (playItem.ScheduleMode == newScheduleMode && playItem.StartTime == newStartTime) return;

            var playRange = playItem.PlayRange;
            //_playlist.ValidateTimeRange(newStartTime, playRange.Duration, playItem);
            ValidateTimeRange(newStartTime, playRange.Duration, playItem);

            // NOTE: 对于顺播PlayRange可能需要改变，因此不能简单的克隆PlaySource。
            var playSource = new PlaySource(playItem.MediaSource/*.Clone()*/, playRange, playItem.CGItems);
            
            var newItem = newScheduleMode == PlayScheduleMode.Timing ?
                PlaybillItem.Timing(playSource, newStartTime) : PlaybillItem.TimingBreak(playSource, newStartTime);

            /*
             * 修改定时播开始时间。不能跨越前后定时播。
             * 修改定时插播开始时间。不能向前跨越第一个定时播。
             */

            if (scheduleMode == null && startTime!=null)
            {
                if (playItem.ScheduleMode == PlayScheduleMode.Timing)
                {
                    // 查找前一个定时播。
                    var prevTuple = this.FindLastTiming(i => i.StartTime <playItem.StartTime/*startTime.Value*/);
   
                    if (prevTuple.Item2 != -1)
                    {
                        // 如果前面有定时播，则开始时间不能小于前面定时播的结束时间。
                        if (startTime.Value < prevTuple.Item1.CalculatedStopTime)
                        {
                            throw new ArgumentException();
                        }
                    }

                    // 查找后一个定时播。
                    var nextTuple = this.FindFirstTiming(i => i.StartTime > playItem.StartTime/*startTime.Value*/);
                    if (nextTuple.Item2 != -1)
                    {
                        // 如果后面有定时播，则结束时间不能大于后面定时播的开始时间。
                        if (startTime.Value.Add(playItem.PlayRange.Duration) > prevTuple.Item1.StartTime)
                        {
                            throw new ArgumentException();
                        }
                    }

                    var currentTuple = this.FindFirstTiming(i => i.PlayItem == playItem);

                    if (currentTuple.Item2 != 0)
                    {
                        // 如果前面有片断，则更新前面片断。
                        var begin1 = prevTuple.Item2 == -1 ? 0 : prevTuple.Item2 + 1;

                        this.Rebuild(_playlist[begin1].StartTime < startTime.Value ? _playlist[begin1].StartTime : startTime.Value, 
                            startTime.Value, begin1, currentTuple.Item2 - 1, null);
                    }

                    // 更新本片断。
                    var temp = (IPlayItem)newItem;
                    _playlist[currentTuple.Item2] = new ScheduleItem(temp);
                    this.Rebuild(temp.CalculatedStopTime,
                            nextTuple.Item2==-1 ? DateTime.MaxValue : nextTuple.Item1.StartTime/*stopTime*/, currentTuple.Item2+1,
                            nextTuple.Item2 == -1 ? _playlist.Count-1 : nextTuple.Item2-1, null);

                    return;
                }
                else if (playItem.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    //var prevTuple = this.FindLastTiming(i => i.StartTime < startTime.Value);

                    //if (prevTuple.Item2 == -1)
                    //{
                    //    if (startTime.Value < prevTuple.Item1.CalculatedStopTime)
                    //    {
                    //        throw new ArgumentException();
                    //    }
                    //}
                }
                else
                {

                }
            }

            this.Delete(playItem);
            this.AddTiming((IPlayItem)newItem);
        }

        /// <summary>
        /// 把定时播或定时插播改为顺播。
        /// </summary>
        /// <param name="playItem"></param>
        private void ChangeToAutoScheduleInternal(IPlayItem playItem)
        {
            Debug.Assert(playItem.ScheduleMode == PlayScheduleMode.Timing || playItem.ScheduleMode == PlayScheduleMode.TimingBreak);

            //if (_playlist.IsLocked(playItem)) throw new PlayItemLockedException();

            var prevTuple = this.FindLastTiming(i => i.StartTime < playItem.StartTime);
            DateTime startTime, stopTime;
            int beginIndex, endIndex;

            if (prevTuple.Item2 == -1)
            {
                startTime = _playlist[0].StartTime;
                beginIndex = 0;
            }
            else
            {
                startTime = prevTuple.Item1.CalculatedStopTime;
                beginIndex = prevTuple.Item2 + 1;
            }

            var nextTuple = this.FindFirstTiming(beginIndex, i => i.StartTime > playItem.StartTime);

            if (nextTuple.Item2 == -1)
            {
                stopTime = DateTime.MaxValue;
                endIndex = _playlist.Count - 1;
            }
            else
            {
                stopTime = nextTuple.Item1.StartTime;
                endIndex = nextTuple.Item2 - 1;
            }

            var playSource = ((PlaybillItem)playItem.PlaybillItem).PlaySource.Clone();
            var newAutoItem = new AutoPlayItem(PlaybillItem.Auto(playSource));

            Rebuild(startTime, stopTime, beginIndex, endIndex, (items) => 
            {
                var i = items.FindIndex(si=>si.PlayItem==playItem);
                items[i] = new ScheduleItem(newAutoItem);
            });
        }

        private void Rebuild(DateTime startTime, DateTime stopTime, int beginIndex, int endIndex,
            Action<List<ScheduleItem>> itemsAction)
        {
            List<ScheduleItem> playItems = GetPlaylistItems(beginIndex, endIndex);

            if (itemsAction != null)
            {
                itemsAction(playItems);
            }

            //PlaylistBuildData data = new PlaylistBuildData(this.Id);
            NewPlaylistBuildData data = new NewPlaylistBuildData();

            data.StartTime = startTime;
            data.StopTime = stopTime;

            for (int i = 0; i < playItems.Count; i++)
            {
                var item = playItems[i];
                if (item.ScheduleMode == PlayScheduleMode.Auto)
                {
                    data.AddAuto(item);
                }
                else
                {
                    data.InsertTiming(item);
                }
            }

            UpdatePlaylist(beginIndex, endIndex - beginIndex + 1, _builder.Build(data));
        }
    }
}
