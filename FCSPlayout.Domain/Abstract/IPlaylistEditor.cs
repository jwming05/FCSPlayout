using System;
using System.Collections.Generic;

namespace FCSPlayout.Domain
{
    public interface IPlaylistEditor : IDisposable
    {
        void AddAutoAfter(IPlayItem prevItem, AutoPlaybillItem newItem);
        void InsertTiming(PlaybillItem billItem);
        void ClearAll();
        void Append(IList<IPlayItem> playItems);
        void Delete(IPlayItem playItem);

        event EventHandler Disposed;

        //void ChangeStartTime(IPlayItem playItem, DateTime newStartTime);
    }
}
