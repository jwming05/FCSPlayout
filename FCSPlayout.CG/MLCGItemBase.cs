using MLCHARGENLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.CG
{
    public abstract class MLCGItemBase : CGItem
    {
        public sealed override string Attach(object obj)
        {
            CoMLCharGen charGen = obj as CoMLCharGen;
            if (charGen == null)
            {
                throw new ArgumentException();
            }

            var id = Guid.NewGuid().ToString("N");
            this.Attach(charGen, id);
            return id;
        }

        public sealed override void Detach(object obj, string id)
        {
            CoMLCharGen charGen = obj as CoMLCharGen;
            if (charGen == null)
            {
                throw new ArgumentException();
            }

            this.Detach(charGen, id);
        }
        protected abstract void Attach(CoMLCharGen charGen, string id);

        protected virtual void Detach(CoMLCharGen charGen, string id)
        {
            charGen.RemoveItem(id, 0);
        }
    }
}
