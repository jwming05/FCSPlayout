using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.PlaybillEditor
{
    public interface IPlayItemEditor:IDisposable
    {
        void ChangePlayRange(IPlayItem playItem, PlayRange newRange);
    }
}
