using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public interface IPlayableItemEditor:IDisposable
    {
        //void ChangePlayRange(IPlayItem playItem, PlayRange newRange);
        void ChangePlayRange(IPlayableItem playableItem, PlayRange newRange);
    }
}
