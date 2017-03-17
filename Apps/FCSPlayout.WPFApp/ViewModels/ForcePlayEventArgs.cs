using System;
using FCSPlayout.Domain;

namespace FCSPlayout.WPFApp.ViewModels
{
    public class ForcePlayEventArgs:EventArgs
    {
        public ForcePlayEventArgs()
        {
        }

        public IPlayItem CurrentPlayItem { get; internal set; }
        public PlayRange CurrentRemainRange { get; internal set; }
        public IPlayItem ForcePlayItem { get; internal set; }
    }
}