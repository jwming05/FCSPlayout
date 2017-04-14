using FCSPlayout.CG;
using MLCHARGENLib;
using MPLATFORMLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FCSPlayout.WPF.Core
{
    public class CGManager:IDisposable
    {
        private Dictionary<CGItem, string> _cgItems = new Dictionary<CGItem, string>();
        private CoMLCharGen _mlCharGen;
        private MPlaylistClass _mplaylist;

        public CGManager(MPlaylistClass mplaylist)
        {
            this._mplaylist = mplaylist;
            _mlCharGen = new CoMLCharGen();
            mplaylist.PluginsAdd(_mlCharGen, 0);
        }

        public void Attach(CGItemCollection cgItems)
        {
            foreach(CGItem item in cgItems)
            {
                Attach(item);
            }
        }

        public void Detach(CGItemCollection cgItems)
        {
            foreach (CGItem item in cgItems)
            {
                Detach(item);
            }
        }

        public void Attach(CGItem item)
        {
            if (!_cgItems.ContainsKey(item))
            {
                string tag = item.Attach(_mlCharGen);
                _cgItems.Add(item, tag);
            }
        }

        public void Detach(CGItem item)
        {
            if (_cgItems.ContainsKey(item))
            {
                item.Detach(_mlCharGen, _cgItems[item]);
                _cgItems.Remove(item);
            }
        }

        public void Dispose()
        {
            if (_mlCharGen != null)
            {
                _mplaylist.PluginsRemove(_mlCharGen);
                Marshal.ReleaseComObject(_mlCharGen);
                _mlCharGen = null;
            }
        }
    }
}
