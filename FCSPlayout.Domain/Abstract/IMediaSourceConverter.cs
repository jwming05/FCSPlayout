using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public interface IMediaSourceConverter
    {
        T ToEntity<T>(IMediaSource mediaSource) where T : IMediaSourceEntity;
        T FromEntity<T>(IMediaSourceEntity entity) where T : IMediaSource;
    }
}
