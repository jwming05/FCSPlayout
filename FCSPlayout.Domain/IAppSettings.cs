using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public interface IAppSettings
    {
        int MediaFileThumbnailWidth { get; }
        string PrimaryMediaFileStoragePath { get; }
        string SecondaryMediaFileStoragePath { get; }
    }
}
