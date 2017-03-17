using FCSPlayout.Domain;
using Prism.Interactivity.InteractionRequest;
using System;

namespace FCSPlayout.WPF.Core
{
    public class EditMediaItemConfirmation : Confirmation
    {
        private string _filePath;
        private PlayRange _playRange;

        public EditMediaItemConfirmation(string filePath, PlayRange playRange)
        {
            _filePath = filePath;
            _playRange = playRange;
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public PlayRange PlayRange
        {
            get { return _playRange; }
            set
            {
                _playRange = value;
            }
        }

        public TimeSpan StartPosition
        {
            get { return _playRange.StartPosition; }
            set
            {
                var duration = _playRange.StopPosition - value;
                _playRange = new PlayRange(value, duration);
            }
        }

        public TimeSpan StopPosition
        {
            get { return _playRange.StopPosition; }
            set
            {
                var duration = value - _playRange.StartPosition;
                _playRange = new PlayRange(_playRange.StartPosition, duration);
            }
        }
    }
}
