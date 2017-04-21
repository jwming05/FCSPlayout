using FCSPlayout.Domain;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public interface IPlayItemEditor:IDisposable
    {
        void ChangePlayRange(IPlayItem playItem, PlayRange newRange);
    }
}
