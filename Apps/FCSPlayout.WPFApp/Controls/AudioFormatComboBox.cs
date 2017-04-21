using MPLATFORMLib;
using System.Collections.Generic;

namespace FCSPlayout.WPFApp
{
    public class AudioFormatComboBox : FormatComboBoxBase
    {
        private Dictionary<string, M_AUD_PROPS> _audPropsCache = new Dictionary<string, M_AUD_PROPS>();

        protected override void OnSelectedFormatChanged()
        {
            M_AUD_PROPS audProps = _audPropsCache[this.SelectedFormat];
            this.MFormatObject.FormatAudioSet(this.FormatType, ref audProps);
        }

        protected override int PopulateFormatItems()
        {
            _audPropsCache.Clear();

            int count;
            MFormatObject.FormatAudioGetCount(this.FormatType, out count);
            M_AUD_PROPS audProps;
            string name;
            int index = 0;
            for (; index < count; index++)
            {
                MFormatObject.FormatAudioGetByIndex(this.FormatType, index, out audProps, out name);
                this.Items.Add(name);

                _audPropsCache.Add(name, audProps);
            }

            if (count > 0)
            {
                MFormatObject.FormatAudioGet(this.FormatType, out audProps, out index, out name);
                return index;
            }
            else
            {
                return -1;
            }
        }
    }
}
