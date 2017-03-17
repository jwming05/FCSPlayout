using BMDSwitcherAPI;
using System;
using System.Runtime.InteropServices;

namespace FCSPlayout.SwitcherManagement
{
    public static class BMDSwitcherExtensions
    {
        public static void IterateInput(this IBMDSwitcher switcher, Action<IBMDSwitcherInput> action)
        {
            // We create input monitors for each input. To do this we iterate over all inputs:
            // This will allow us to update the combo boxes when input names change:

            IntPtr inputIteratorPtr;
            Guid inputIteratorIID = typeof(IBMDSwitcherInputIterator).GUID;
            switcher.CreateIterator(ref inputIteratorIID, out inputIteratorPtr);

            IBMDSwitcherInputIterator inputIterator = null;
            if (inputIteratorPtr != null)
            {
                inputIterator = (IBMDSwitcherInputIterator)Marshal.GetObjectForIUnknown(inputIteratorPtr);
            }

            if (inputIterator != null)
            {
                IBMDSwitcherInput input;
                inputIterator.Next(out input);
                while (input != null)
                {
                    action(input);

                    inputIterator.Next(out input);
                }
            }
        }

        public static void IterateMixEffectBlock(this IBMDSwitcher switcher,Func<IBMDSwitcherMixEffectBlock,bool> func)
        {
            // We want to get the first Mix Effect block (ME 1). We create a ME iterator,
            // and then get the first one:
            IBMDSwitcherMixEffectBlock mixEffectBlock1 = null;

            IBMDSwitcherMixEffectBlockIterator meIterator = null;
            IntPtr meIteratorPtr;
            Guid meIteratorIID = typeof(IBMDSwitcherMixEffectBlockIterator).GUID;
            switcher.CreateIterator(ref meIteratorIID, out meIteratorPtr);
            if (meIteratorPtr != null)
            {
                meIterator = (IBMDSwitcherMixEffectBlockIterator)Marshal.GetObjectForIUnknown(meIteratorPtr);
            }

            if (meIterator != null)
            {
                meIterator.Next(out mixEffectBlock1);

                while (mixEffectBlock1 != null && func(mixEffectBlock1))
                {
                    meIterator.Next(out mixEffectBlock1);
                }
            }
        }

        public static IBMDSwitcherMixEffectBlock GetFirstMixEffectBlock(this IBMDSwitcher switcher)
        {
            IBMDSwitcherMixEffectBlock result = null;
            switcher.IterateMixEffectBlock((b)=> { result = b;return false; });
            return result;
        }

        // Interface
        public static IBMDSwitcherAudioMixer GetBMDSwitcherAudioMixer(this IBMDSwitcher switcher)
        {
            return switcher.QueryInterface<IBMDSwitcherAudioMixer>();
        }

        public static T QueryInterface<T>(this IBMDSwitcher switcher)
        {
            IntPtr ptr = Marshal.GetIUnknownForObject(switcher);

            Guid guid = typeof(T).GUID;
            IntPtr pUnk = IntPtr.Zero;
            Marshal.QueryInterface(ptr, ref guid, out pUnk);

            return (T)Marshal.GetObjectForIUnknown(pUnk);
        }

        public static void IterateAudioInput(this IBMDSwitcherAudioMixer switcher, Action<IBMDSwitcherAudioInput> action)
        {
            // We create input monitors for each input. To do this we iterate over all inputs:
            // This will allow us to update the combo boxes when input names change:

            IntPtr inputIteratorPtr;
            Guid inputIteratorIID = typeof(IBMDSwitcherAudioInputIterator).GUID;
            switcher.CreateIterator(ref inputIteratorIID, out inputIteratorPtr);

            IBMDSwitcherAudioInputIterator inputIterator = null;
            if (inputIteratorPtr != null)
            {
                inputIterator = (IBMDSwitcherAudioInputIterator)Marshal.GetObjectForIUnknown(inputIteratorPtr);
            }

            if (inputIterator != null)
            {
                IBMDSwitcherAudioInput input;
                inputIterator.Next(out input);
                while (input != null)
                {
                    action(input);
                    inputIterator.Next(out input);
                }
            }
        }
    }
}
