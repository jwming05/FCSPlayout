using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public interface IFileMediaSource : IMediaSource
    {
        string FileName { get; }
        int AudioGain { get; set; }
    }
}
