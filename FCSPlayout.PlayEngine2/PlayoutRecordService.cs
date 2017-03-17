using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.PlayEngine
{
    public abstract class PlayoutRecordService
    {
        private static PlayoutRecordService _current;

        public static PlayoutRecordService Current
        {
            get
            {
                return _current;
            }

            set
            {
                _current = value;
            }
        }

        public abstract void Add(IPlayerItem playerItem);
    }
}
