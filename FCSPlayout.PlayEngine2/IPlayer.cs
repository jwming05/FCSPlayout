﻿using FCSPlayout.CG;
using System;

namespace FCSPlayout.PlayEngine
{
    public interface IPlayer
    {
        void Start();
        void Stop();
        void OnTimer();

        void Load(IPlayerItem playerItem);
        void PlayNext();
        //void StopCurrent();

        void Attach(CGItemCollection cgItems);

        void Detach(CGItemCollection cgItems);

        event EventHandler<PlayerItemEventArgs> ItemLoaded;
        event EventHandler<PlayerItemEventArgs> ItemStarted;
        event EventHandler<PlayerItemEventArgs> ItemStopped;
    }
}
