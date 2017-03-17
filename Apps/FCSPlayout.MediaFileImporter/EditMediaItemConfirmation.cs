using FCSPlayout.Domain;
using Prism.Interactivity.InteractionRequest;
using System;

namespace FCSPlayout.MediaFileImporter
{
    public class EditMediaItemConfirmation : Confirmation
    {
        private PlayRange _playRange;
        private readonly IMediaSource _source;

        public EditMediaItemConfirmation(MediaItem mediaItem)
        {
            _source = mediaItem.Source;
            _playRange = mediaItem.PlayRange;
        }

        public IMediaSource Source
        {
            get { return _source; }
        }

        public PlayRange PlayRange
        {
            get { return _playRange; }
            set
            {
                _playRange = value;

                //if (_playRange.MaxDuration == value.MaxDuration)
                //{
                //    _playRange = value;
                //}
                //else
                //{
                //    _playRange=new PlayRange(_playRange.MaxDuration, value.StartPosition, value.StopPosition);
                //}
            }
        }

        public TimeSpan StartPosition
        {
            get { return _playRange.StartPosition; }
            set
            {
                var stopPos = _playRange.StopPosition;
                if(value>=TimeSpan.Zero && value <= stopPos)
                {
                    _playRange = new PlayRange(value,stopPos-value);
                }
                //_playRange =_playRange.ModifyByStartPosition(value);
            }
        }

        public TimeSpan StopPosition
        {
            get { return _playRange.StopPosition; }
            set
            {
                var startPos = _playRange.StartPosition;
                if (value >= TimeSpan.Zero && value >= startPos)
                {
                    _playRange = new PlayRange(startPos, value - startPos);
                }

                //_playRange = _playRange.ModifyByStopPosition(value);
            }
        }
    }
}
