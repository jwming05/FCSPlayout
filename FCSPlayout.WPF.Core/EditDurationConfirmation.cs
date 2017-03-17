using FCSPlayout.Domain;
using Prism.Interactivity.InteractionRequest;
using System;

namespace FCSPlayout.WPF.Core
{
    public class EditDurationConfirmation : Confirmation
    {
        private readonly IMediaSource _source;
        private TimeSpan _duration;

        public EditDurationConfirmation(MediaItem mediaItem)
        {
            _source = mediaItem.Source;
            _duration = mediaItem.PlayRange.Duration;
        }

        public IMediaSource Source
        {
            get { return _source; }
        }

        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;
            }
        }
    }
}
