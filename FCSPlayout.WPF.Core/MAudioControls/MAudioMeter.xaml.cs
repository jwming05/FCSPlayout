using MPLATFORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// MAudioMeter.xaml 的交互逻辑
    /// </summary>
    public partial class MAudioMeter : UserControl
    {
        private int m_nCurrentTrack = 0;
        private IMAudio m_pMAudio;
        private IMAudioTrack m_pMAudioTrack;

        // This is for shorter combox-box: 'eMAT_Mix_Exclusive' - too long
        private eMAudioTrackMode[] pModes = new eMAudioTrackMode[6]
            {
                eMAudioTrackMode.eMAT_Enabled,
                eMAudioTrackMode.eMAT_Disabled,
                eMAudioTrackMode.eMAT_Exclusive,
                eMAudioTrackMode.eMAT_Mix_Exclusive,
                eMAudioTrackMode.eMAT_Enabled_AddTo,
                eMAudioTrackMode.eMAT_Disabled_AddTo
            };

        private DispatcherTimer _timerUpdate;
        public MAudioMeter()
        {
            InitializeComponent();

            comboBoxMode.Items.Add("Enabled");
            comboBoxMode.Items.Add("Disabled");
            comboBoxMode.Items.Add("Exclusive");
            comboBoxMode.Items.Add("Mix_Exclusive");
            // Next modes not implemented (in control)
            //comboBoxMode.Items.Add(eMAudioTrackMode.eMAT_Enabled_AddTo);
            //comboBoxMode.Items.Add(eMAudioTrackMode.eMAT_Disabled_AddTo);

            _timerUpdate = new DispatcherTimer();
            _timerUpdate.Interval = TimeSpan.FromMilliseconds(100);
            _timerUpdate.Tick += timerUpdate_Tick;
        }

        public Object SetControlledObject(Object pObject)
        {
            Object pOld = (Object)m_pMAudio;
            m_pMAudio = null;
            if (pObject != null)
            {
                try
                {
                    m_pMAudio = (IMAudio)pObject;

                    FillTracks();
                    UpdateControl(m_nCurrentTrack);

                    _timerUpdate.Start();
                }
                catch (System.Exception)
                {
                    _timerUpdate.Stop();
                    this.IsEnabled = false;
                }
            }
            else
            {
                _timerUpdate.Stop();
                this.IsEnabled = false;
            }

            return pOld;
        }

        #region
        private int GetAudioTracksCount()
        {
            int nCount = 0;
            m_pMAudio.AudioTracksGetCount(out nCount);
            return nCount;
        }

        private string GetAudioTrackNameByIndex(int index)
        {
            string sName;
            IMAudioTrack pTrack;
            m_pMAudio.AudioTrackGetByIndex(index, out sName, out pTrack);
            if (pTrack != null)
            {
                Marshal.ReleaseComObject(pTrack);
            }
            return sName;
        }

        private IMAudioTrack GetAudioTrackByIndex(int index)
        {
            string strTrack;
            IMAudioTrack pMAudioTrack = null;
            m_pMAudio.AudioTrackGetByIndex(index, out strTrack, out pMAudioTrack);
            return pMAudioTrack;
        }

        private static int GetTrackOutputChannels(IMAudioTrack pMAudioTrack)
        {
            int nChIn, nChOut, nChOutIdx;
            pMAudioTrack.TrackChannelsGet(out nChIn, out nChOutIdx, out nChOut);
            return nChOut;
        }

        private static eMAudioTrackMode GetTrackMode(IMAudioTrack pMAudioTrack)
        {
            int nAdd;
            double dblAddGain;
            eMAudioTrackMode eMode;
            pMAudioTrack.TrackModeGet(out eMode, out nAdd, out dblAddGain);
            return eMode;
        }

        private static void SetTrackGain(IMAudioTrack pMAudioTrack, int nChannel, double gain)
        {
            pMAudioTrack.TrackGainSet(nChannel, gain, 0.1);
        }

        private static void SetTrackMute(IMAudioTrack pMAudioTrack, int nChannel, bool mute)
        {
            pMAudioTrack.TrackMuteSet(nChannel, mute ? 1 : 0, 0.1);
        }

        

        private static bool GetTrackMute(IMAudioTrack pMAudioTrack, int channel)
        {
            int nMute = 0;
            pMAudioTrack.TrackMuteGet(channel, out nMute);
            return nMute == 1;
        }

        private static double GetTrackGain(IMAudioTrack pMAudioTrack, int channel)
        {
            double dblGain = 0;
            pMAudioTrack.TrackGainGet(channel, out dblGain);
            return dblGain;
        }

        private static void GetTrackLoudness(IMAudioTrack pMAudioTrack, out M_AUDIO_TRACK_LOUDNESS loudOrg, out M_AUDIO_TRACK_LOUDNESS loudOut)
        {
            pMAudioTrack.TrackLoudnessGet(out loudOrg, out loudOut);
        }
        #endregion
        private void FillTracks()
        {
            if (m_pMAudio != null)
            {
                comboBoxTrack.Items.Clear();

                int nCount = GetAudioTracksCount();
                for (int i = 0; i < nCount; i++)
                {
                    string sName= GetAudioTrackNameByIndex(i);
                    comboBoxTrack.Items.Add(sName);
                }

                comboBoxTrack.SelectedIndex = m_nCurrentTrack;
            }
        }



        private void UpdateControl(int trackIndex)
        {
            if (m_pMAudio != null)
            {

                //if (m_pMAudioTrack != null)
                //{
                //    Marshal.ReleaseComObject(m_pMAudioTrack);
                //    m_pMAudioTrack = null;
                //}

                try
                {
                    m_pMAudioTrack = GetAudioTrackByIndex(trackIndex);

                    // Get number of channels
                    int nChOut =GetTrackOutputChannels(m_pMAudioTrack);

                    // Update track mode
                    eMAudioTrackMode eMode=GetTrackMode(m_pMAudioTrack);

                    var modeText = eMode.ToString().Substring(5);
                    for(int i = 0; i < comboBoxMode.Items.Count; i++)
                    {
                        if (object.Equals(modeText, comboBoxMode.Items[i]))
                        {
                            comboBoxMode.SelectedIndex = i;
                            break;
                        }
                    }                    

                    // if track disabled -> 0 real track channels
                    // TODO: Exclusive modes
                    if (eMode == eMAudioTrackMode.eMAT_Disabled || eMode == eMAudioTrackMode.eMAT_Disabled_AddTo)
                    {
                        nChOut = 0;
                    }

                    channelPanel.Children.Clear();

                    for (int i = 0; i < nChOut; i++)
                    {
                        var channelControl = CreateChannelControl(i);
                        channelPanel.Children.Add(channelControl);
                    }

                    m_nCurrentTrack = trackIndex;

                    this.InvalidateVisual();
                }
                catch (System.Exception ex)
                {

                }
            }
        }

        

        private MAudioChannel CreateChannelControl(int i)
        {
            var channelControl = new MAudioChannel();
            //channelControl.ColorLevelBack = ColorLevelBack;
            //channelControl.ColorLevelOrg = ColorLevelOrg;
            //channelControl.ColorLevelHi = ColorLevelHi;
            //channelControl.ColorLevelMid = ColorLevelMid;
            //channelControl.ColorLevelLo = ColorLevelLo;
            //channelControl.ColorOutline = ColorOutline;
            //channelControl.ColorGainSlider = ColorGainSlider;
            //channelControl.Risk = 6;

            channelControl.GainChanged += new RoutedEventHandler(MAudioControl_OnGainChanged);
            channelControl.ChannelEnabledChanged += new RoutedEventHandler(MAudioMeter_OnEnableChanged);

            double dblGain = 0;
            bool bMute = true;
            try
            {
                // The channels could be changed in this moment.
                dblGain = GetTrackGain(m_pMAudioTrack, i);
                bMute = GetTrackMute(m_pMAudioTrack, i);
            }
            catch (System.Exception)
            {

            }

            channelControl.Gain = dblGain;
            channelControl.ChannelEnabled = !bMute;

            return channelControl;
        }

        void MAudioControl_OnGainChanged(object sender, EventArgs e)
        {
            int nChannel = Control2ChannelIndex(sender);
            if (nChannel >= 0)
            {
                bool bAllChange = false;
                try
                {
                    // By right button chnage all channels gain
                    if (Mouse.RightButton == MouseButtonState.Pressed)
                    {
                        bAllChange = true;
                    }
                }
                catch (System.Exception) { }

                SetTrackGain(m_pMAudioTrack, bAllChange ? -1 : nChannel, ((MAudioChannel)channelPanel.Children[nChannel]).Gain);
                if (bAllChange)
                {
                    for (int i = 0; i < channelPanel.Children.Count; i++)
                    {
                        double dblGain = GetTrackGain(m_pMAudioTrack, i);
                        ((MAudioChannel)channelPanel.Children[i]).Gain = dblGain;
                    }
                }
            }
        }

        void MAudioMeter_OnEnableChanged(object sender, EventArgs e)
        {
            int nChannel = Control2ChannelIndex(sender);
            if (nChannel >= 0)
            {
                SetTrackMute(m_pMAudioTrack, nChannel,!((MAudioChannel)channelPanel.Children[nChannel]).ChannelEnabled);
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (m_pMAudio == null) return;

            // Check number of tracks
            int nTracks = GetAudioTracksCount();
            if (nTracks <= 0)
            {
                this.IsEnabled = false;
                //m_pMAudioTrack = null;
                return;
            }

            this.IsEnabled = true;

            if (comboBoxTrack.Items.Count != nTracks)
            {
                m_nCurrentTrack = Math.Min(nTracks - 1, m_nCurrentTrack);
                FillTracks();
                UpdateControl(m_nCurrentTrack);
            }


            if (m_pMAudioTrack != null)
            {
                try
                {
                    // TODO: use loudOut.nValidChannels
                    //int nChIn, nChOut, nChOutIdx;
                    //m_pMAudioTrack.TrackChannelsGet(out nChIn, out nChOutIdx, out nChOut);
                    int nChOut=GetTrackOutputChannels(m_pMAudioTrack);
                    if (nChOut !=channelPanel.Children.Count)
                    {
                        UpdateControl(comboBoxTrack.SelectedIndex);
                    }


                    M_AUDIO_TRACK_LOUDNESS loudOrg;
                    M_AUDIO_TRACK_LOUDNESS loudOut;
                    GetTrackLoudness(m_pMAudioTrack, out loudOrg, out loudOut);

                    // Get number of channels
                    //int nChIn, nChOut, nChOutIdx;
                    //m_pMAudioTrack.TrackChannelsGet(out nChIn, out nChOutIdx, out nChOut);

                    //int nChOut=GetTrackOutputChannels(m_pMAudioTrack);
                    //if (nChOut != arrChannels.Length)
                    //{
                    //    UpdateControl(comboBoxTrack.SelectedIndex);
                    //}

                    
                    for (int i = 0; i < channelPanel.Children.Count; i++)
                    {
                        
                        MAudioChannel channelControl = (MAudioChannel)channelPanel.Children[i];
                        double gain = GetTrackGain(m_pMAudioTrack, i);
                        if (gain != channelControl.Gain)
                        {
                            channelControl.Gain = gain;
                        }

                        if (i >= loudOut.nValidChannels)
                        {
                            channelControl.Level = -60;
                            channelControl.LevelOrg = -60;
                            channelControl.Refresh();
                            //arrChannels[i].Invalidate();
                            channelControl.IsEnabled = false;
                        }
                        else
                        {
                            channelControl.Level = loudOut.arrVUMeter[i];
                            channelControl.LevelOrg = loudOrg.arrVUMeter[i]; // Original
                            channelControl.Refresh();
                            //arrChannels[i].Invalidate();
                            channelControl.IsEnabled = true;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // If number of tracks changed or e.g. playlist loaded -> the current track may be not valid
                    UpdateControl(comboBoxTrack.SelectedIndex);
                }
            }
        }

        private int Control2ChannelIndex(object sender)
        {
            for (int i = 0; i <channelPanel.Children.Count; i++)
            {
                if (channelPanel.Children[i].Equals(sender))
                {
                    return i;
                }
            }
            return -1;
        }

        private void comboBoxTrack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_nCurrentTrack != comboBoxTrack.SelectedIndex)
            {
                UpdateControl(comboBoxTrack.SelectedIndex);
            }
        }

        private void comboBoxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_pMAudioTrack != null)
            {
                m_pMAudioTrack.TrackModeSet(pModes[comboBoxMode.SelectedIndex], 0, 0.0);
            }
        }
    }
}
