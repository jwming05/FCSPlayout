using FCSPlayout.CG;
using System;
namespace FCSPlayout.Domain
{
    public interface IPlaySource
    {
        /// <summary>
        /// 获取媒体源。
        /// </summary>
        IMediaSource MediaSource { get; }

        /// <summary>
        /// 获取应用于媒体源的入出点。
        /// </summary>
        PlayRange PlayRange { get; }

        //IPlayParameters Parameters { get; }

        string Title { get; }

        CGItemCollection CGItems { get; set; }

        IPlaySource Clone();

        IPlaySource Clone(PlayRange newRange);
        bool CanMerge(IPlaySource playSource);
        IPlaySource Merge(IPlaySource playSource);
    }
}