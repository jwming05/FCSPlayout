using FCSPlayout.Domain;
using System;

namespace FCSPlayout.WPF.Core
{
    public interface IPlayableItem
    {
        int AudioGain { get; set; }
        string FilePath { get; }
        PlayRange PlayRange { get; set; }

        void ClosePreview();
        event EventHandler PreviewClosing;

        IPlayItem PlayItem { get; }
    }

    public interface IPlayableItemWithCG:IPlayableItem
    {
        CG.CGItemCollection CGItems { get; }
    }
}
