using MPLATFORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FCSPlayout.WPFApp
{
    public class VideoFormatComboBox : FormatComboBoxBase
    {
        private Dictionary<string, M_VID_PROPS> _vidPropsCache = new Dictionary<string, M_VID_PROPS>();

        protected override void OnSelectedFormatChanged()
        {
            M_VID_PROPS vidProps = _vidPropsCache[this.SelectedFormat];
            this.MFormatObject.FormatVideoSet(this.FormatType, ref vidProps);
        }

        protected override int PopulateFormatItems()
        {
            _vidPropsCache.Clear();

            int count;
            MFormatObject.FormatVideoGetCount(this.FormatType, out count);
            M_VID_PROPS vidProps;
            string name;
            int index = 0;
            for (; index < count; index++)
            {
                MFormatObject.FormatVideoGetByIndex(this.FormatType, index, out vidProps, out name);
                this.Items.Add(name);

                _vidPropsCache.Add(name, vidProps);
            }

            if (count > 0)
            {
                MFormatObject.FormatVideoGet(this.FormatType, out vidProps, out index, out name);
                return index;
            }
            else
            {
                return -1;
            }
        }
    }
}
