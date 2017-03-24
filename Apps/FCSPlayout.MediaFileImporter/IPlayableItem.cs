using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    public interface IPlayableItem
    {
        int AudioGain { get; set; }
        string FilePath { get; }
        PlayRange PlayRange { get; set; }
    }
}
