using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FCSPlayout.Domain
{
    public class PlaylistBuilder
    {
        public IList<IPlayItem> Build(PlaylistBuildData data)
        {
            AutoPlayItem autoItem =null;
            IPlayItem timingItem = null;
            AutoPlayItem segmentItem = null;
            bool isSegment = false;

            DateTime startTime = data.StartTime;

            List<IPlayItem> result = new List<IPlayItem>();

            while (startTime<data.StopTime)
            {
                if(autoItem==null)
                {
                    if (segmentItem != null)
                    {
                        autoItem = segmentItem;
                        segmentItem = null;
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
                            if (maxDuration >= autoItem.PlayDuration)
                            {
                                result.Add(autoItem);
                                startTime = autoItem.GetStopTime();
                            }
                            else
                            {
                                if (timingItem.PlaybillItem.Category == PlaybillItemCategory.Timing)
                                {
                                    // 截短。
                                    autoItem.PlayDuration = maxDuration;
                                    result.Add(autoItem);
                                    startTime = autoItem.GetStopTime();
                                }
                                else
                                {
                                    // 分片。

                                    AutoPlayItem first = null;
                                    AutoPlayItem second = null;
                                    autoItem.Split(maxDuration, out first, out second);
                                    first.StartTime = startTime;
                                    result.Add(first);
                                    startTime = first.GetStopTime();

                                    segmentItem = second;
                                    
                                    //autoItem = second;
                                }
                                
                            }
                            autoItem = null;
                        }
                        else
                        {
                            // 插入自动垫片。
                            IPlayItem autoPadding = CreateAutoPadding(startTime, maxDuration);
                            result.Add(autoPadding);
                        }
                        #endregion 1.1
                    }
                    else
                    {
                        #region 1.2
                        Debug.Assert(startTime == timingItem.StartTime);

                        if (timingItem.PlaybillItem.Category == PlaybillItemCategory.Timing && autoItem != null && isSegment)
                        {
                            // 完全截断。
                            autoItem.StartTime = startTime;
                            autoItem.PlayDuration = TimeSpan.Zero;
                            result.Add(autoItem);

                            autoItem = null;
                        }

                        result.Add(timingItem);
                        startTime = timingItem.GetStopTime();
                        timingItem = null;
                        #endregion 1.2
                    }
                    #endregion 1

                }
                else if(autoItem!=null)
                {
                    #region 2
                    #endregion 2
                    var maxDuration = data.StopTime.Subtract(startTime);
                    autoItem.StartTime = startTime;
                    if (autoItem.PlayDuration > maxDuration)
                    {
                        // 截短。
                        autoItem.PlayDuration = maxDuration;
                    }
                    startTime = autoItem.GetStopTime();
                    autoItem = null;
                }
                else
                {
                    #region 3
                    if (data.StopTime != DateTime.MaxValue && startTime < data.StopTime)
                    {
                        // 插入自动垫片。
                        TimeSpan duration = data.StopTime.Subtract(startTime);
                        IPlayItem autoPadding = CreateAutoPadding(startTime, duration);
                        result.Add(autoPadding);
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
                autoItem.PlayDuration = TimeSpan.Zero;

                result.Add(autoItem);
                autoItem = null;
            }

            while (data.HasAutoPlaybillItem)
            {
                // 完全截断。
                autoItem = data.TakeAuto();

                autoItem.StartTime = data.StopTime;
                autoItem.PlayDuration = TimeSpan.Zero;

                result.Add(autoItem);
                autoItem = null;
            }

            return result;
        }

        private IPlayItem CreateAutoPadding(DateTime startTime,TimeSpan duration)
        {
            return AutoPlayItem.CreateAutoPadding(startTime,duration);
        }
    }
}
