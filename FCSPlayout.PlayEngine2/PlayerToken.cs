using FCSPlayout.CG;
using FCSPlayout.Domain;
using MPLATFORMLib;
using System;
using System.Runtime.InteropServices;

namespace FCSPlayout.PlayEngine
{
    class PlayerToken : IPlayerToken, IDisposable
    {
        //private static IdGenerator idGenerator = new IdGenerator();

        private IDateTimeService _dateTimeService;

        public event EventHandler Stopped;

        public PlayerToken(IPlayerItem playerItem,IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
            //this.Id = idGenerator.Generate();
            this.MediaSource = playerItem.MediaSource;
            this.PlayRange = playerItem.PlayRange;
            //this.Parameters = playerItem.PlayParameters;

            this.Position = TimeSpan.Zero;
            this.RemainTime = this.PlayRange.Duration;

            this.CGItems = playerItem.CGItems;
        }

        //internal string Id { get; private set; }

        public CGItemCollection CGItems { get; private set; }

        public void Dispose()
        {
            if (this.MItem != null)
            {
                Marshal.ReleaseComObject(this.MItem);
                this.MItem = null;
            }
        }

        public TimeSpan Position
        {
            get;private set;
        }

        public DateTime StartTime { get; internal set; }
        
        public void OnTimer()
        {
            if (this.MItem != null)
            {
                eMState mstate;
                double dblRemain;

                this.MItem.FileStateGet(out mstate, out dblRemain);

                if (mstate == eMState.eMS_Closed)
                {
                    return;
                }

                if (mstate == eMState.eMS_Running)
                {
                    double dblPos;
                    this.MItem.FilePosGet(out dblPos);

                    this.RemainTime = TimeSpan.FromSeconds(dblRemain);
                    this.Position = TimeSpan.FromSeconds(dblPos);
                }
                else if(mstate!=eMState.eMS_Paused) // Error, Break, Stopped
                {
                    this.RemainTime = TimeSpan.Zero;
                    this.Position = this.LoadRange.Duration;
                    OnStopped();
                }
            }
            else
            {
                var now = _dateTimeService.GetLocalNow();
                var stopTime = this.StartTime.Add(this.LoadRange.Duration);
                if (now >= stopTime)
                {
                    this.RemainTime = TimeSpan.Zero;
                    this.Position = this.LoadRange.Duration;
                    OnStopped();
                }
                else
                {
                    this.Position = now.Subtract(this.StartTime);
                    this.RemainTime = this.LoadRange.Duration - this.Position;
                }
            }
        }

        public override string ToString()
        {
            return this.MediaSource.ToString();
        }

        public TimeSpan RemainTime
        {
            get;private set;
        }

        public PlayRange LoadRange { get; internal set; }

        internal IMediaSource MediaSource
        {
            get;private set;
        }

        //internal IPlayParameters Parameters
        //{
        //    get;private set;
        //}

        internal PlayRange PlayRange
        {
            get;private set;
        }

        internal MItem MItem { get; set; }

        internal TimeSpan PlayDuration { get { return this.PlayRange.Duration; } }

        internal bool HasAudio { get; set; }

        private void OnStopped()
        {
            if (Stopped != null)
            {
                Stopped(this, EventArgs.Empty);
            }
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine("[HGL] " + message + this);
        }
    }
}
