using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FCSPlayout.Domain
{
    public class PlaylistSegmentBuilder:IPlaylistSegmentBuilder
    {
        private IList<ScheduleItem> _scheduleItems;

        public PlaylistSegmentBuilder(PlaylistSegmentBuildData data)
        {
            this.Build(data);
            _scheduleItems = data.Result;
        }

        public IEnumerable<IPlayItem> Build()
        {
            foreach (ScheduleItem scheduleItem in _scheduleItems)
            {
                yield return scheduleItem.CommitChange();
            }
        }

        private void Build(PlaylistSegmentBuildData data)
        {
            ScheduleItem autoItem =null;
            ScheduleItem timingBreakItem = null;

            DateTime startTime = data.StartTime;

            ScheduleItem timingItem = data.TakeTiming();
            if (timingItem!=null)
            {
                data.AddResult(timingItem);
                startTime = timingItem.CalculatedStopTime;
            }

            autoItem = data.TakeAuto();
            timingBreakItem = data.TakeTimingBreak();

            while (startTime<data.StopTime && (autoItem!=null || timingBreakItem!=null))
            {
                if (timingBreakItem != null)
                {

                    #region 1
                    if (startTime < timingBreakItem.StartTime)
                    {
                        #region 1.1
                        TimeSpan maxDuration = timingBreakItem.StartTime.Subtract(startTime);
                        if (autoItem != null)
                        {
                            autoItem.StartTime = startTime;
                            if (maxDuration >= autoItem.CalculatedPlayDuration)
                            {
                                data.AddResult(autoItem);
                                startTime = autoItem.CalculatedStopTime;

                                autoItem = data.TakeAuto();
                            }
                            else
                            {
                                // 分片。
                                ScheduleItem first = null;
                                ScheduleItem second = null;
                                autoItem.Split(maxDuration, out first, out second);
                                first.StartTime = startTime;
                                data.AddResult(first);
                                startTime = first.CalculatedStopTime;
                                autoItem = second;
                            }
                        }
                        else
                        {
                            // 插入自动垫片。
                            ScheduleItem autoPadding = CreateAutoPadding(startTime, maxDuration);
                            data.AddResult(autoPadding);
                            startTime = autoPadding.CalculatedStopTime;
                        }
                        #endregion 1.1
                    }
                    else
                    {
                        // 插入定时插播。
                        #region 1.2
                        Debug.Assert(startTime == timingBreakItem.StartTime);

                        data.AddResult(timingBreakItem);
                        startTime = timingBreakItem.CalculatedStopTime;
                        timingBreakItem = data.TakeTimingBreak();
                        #endregion 1.2
                    }
                    #endregion 1

                }
                else
                {
                    #region 2
                    var maxDuration = data.StopTime.Subtract(startTime);
                    autoItem.StartTime = startTime;
                    if (autoItem.CalculatedPlayDuration > maxDuration)
                    {
                        // 截短。
                        autoItem.CalculatedPlayDuration = maxDuration;
                    }
                    data.AddResult(autoItem);
                    startTime = autoItem.CalculatedStopTime;
                    autoItem = data.TakeAuto();
                    #endregion 2
                }
            }

            // startTime>=data.StopTime || (autoItem==null && timingBreakItem==null)
            Debug.Assert(timingBreakItem==null);

            while (autoItem != null)
            {
                // 完全截断。
                autoItem.StartTime = data.StopTime;
                autoItem.CalculatedPlayDuration = TimeSpan.Zero;

                data.AddResult(autoItem);
                autoItem = data.TakeAuto();
            }

            if (startTime < data.StopTime && data.StopTime!=DateTime.MaxValue)
            {
                // 插入自动垫片。
                ScheduleItem autoPadding = CreateAutoPadding(startTime, data.StopTime.Subtract(startTime));
                data.AddResult(autoPadding);
                startTime = autoPadding.CalculatedStopTime;
            }

            //return data.Result;
        }

        private ScheduleItem CreateAutoPadding(DateTime startTime,TimeSpan duration)
        {
            return new ScheduleItem(AutoPlayItem.CreateAutoPadding(startTime,duration));
        }
    }
}
