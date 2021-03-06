﻿using System;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class InvalidTimeException:Exception
    {
        public InvalidTimeException(DateTime time)
        {
            this.Time = time;
        }

        public DateTime Time { get; private set; }
    }
}
