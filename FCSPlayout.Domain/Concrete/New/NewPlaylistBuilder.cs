using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FCSPlayout.Domain
{
    public class NewPlaylistBuilder
    {
        public IList<ScheduleItem> Build(NewPlaylistBuildData data)
        {
            ScheduleItem autoItem =null;
            ScheduleItem timingItem = null;
            ScheduleItem partialAutoItem = null;
            bool isSegment = false;

            DateTime startTime = data.StartTime;

            while (startTime<data.StopTime)
            {
                if(autoItem==null)
                {
                    if (partialAutoItem != null)
                    {
                        autoItem = partialAutoItem;
                        partialAutoItem = null;
                        isSegment = true;
                    }
                    else if (data.HasAutoPlaybillItem)
                    {
                        autoItem = data.TakeAuto();
                        isSegment = false;
                    }
                    else
                    {
                        isSegment = false;
                    }
                }

                if(timingItem==null && data.HasTimingPlaybillItem)
                {
                    timingItem = data.TakeTiming();
                }

                if (timingItem != null)
                {
                    #region 1
                    if (startTime < timingItem.StartTime)
                    {
                        #region 1.1
                        TimeSpan maxDuration = timingItem.StartTime.Subtract(startTime);
                        if (autoItem != null)
                        {
                            autoItem.StartTime = startTime;
                            if (maxDuration >= autoItem.CalculatedPlayDuration)
                            {
                                data.AddResult(autoItem);
                                startTime = autoItem.CalculatedStopTime;
                            }
                            else
                            {
                                if (timingItem.ScheduleMode == PlayScheduleMode.Timing)
                                {
                                    // 截短。
                                    autoItem.CalculatedPlayDuration = maxDuration;
                                    data.AddResult(autoItem);
                                    startTime = autoItem.CalculatedStopTime;
                                }
                                else
                                {
                                    // 分片。

                                    ScheduleItem first = null;
                                    ScheduleItem second = null;
                                    autoItem.Split(maxDuration, out first, out second);
                                    //first.Editor = autoItem.Editor;
                                    //second.Editor = autoItem.Editor;

                                    first.StartTime = startTime;
                                    data.AddResult(first);
                                    startTime = first.CalculatedStopTime;

                                    partialAutoItem = second;
                                    
                                    //autoItem = second;
                                }
                                
                            }
                            autoItem = null;
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
                        #region 1.2
                        Debug.Assert(startTime == timingItem.StartTime);

                        if (timingItem.ScheduleMode == PlayScheduleMode.Timing && autoItem != null && isSegment)
                        {
                            // 完全截断。
                            autoItem.StartTime = startTime;
                            autoItem.CalculatedPlayDuration = TimeSpan.Zero;
                            data.AddResult(autoItem);

                            autoItem = null;
                        }

                        data.AddResult(timingItem);
                        startTime = timingItem.CalculatedStopTime;
                        timingItem = null;
                        #endregion 1.2
                    }
                    #endregion 1

                }
                else if(autoItem!=null)
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
                    autoItem = null;
                    #endregion 2

                }
                else
                {
                    #region 3
                    if (data.StopTime != DateTime.MaxValue && startTime < data.StopTime)
                    {
                        // 插入自动垫片。
                        TimeSpan duration = data.StopTime.Subtract(startTime);
                        ScheduleItem autoPadding = CreateAutoPadding(startTime, duration);
                        data.AddResult(autoPadding);
                    }
                    startTime = data.StopTime;
                    #endregion 3
                }
            }

            Debug.Assert(!data.HasTimingPlaybillItem);

            if (autoItem != null)
            {
                // 完全截断。
                autoItem.StartTime = data.StopTime;
                autoItem.CalculatedPlayDuration = TimeSpan.Zero;

                data.AddResult(autoItem);
                autoItem = null;
            }

            while (data.HasAutoPlaybillItem)
            {
                // 完全截断。
                autoItem = data.TakeAuto();

                autoItem.StartTime = data.StopTime;
                autoItem.CalculatedPlayDuration = TimeSpan.Zero;

                data.AddResult(autoItem);
                autoItem = null;
            }

            return data.Result;
        }

        private ScheduleItem CreateAutoPadding(DateTime startTime,TimeSpan duration)
        {
            return new ScheduleItem(AutoPlayItem.CreateAutoPadding(startTime,duration));
        }
    }
}
