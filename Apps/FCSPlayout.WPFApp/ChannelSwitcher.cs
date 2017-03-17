using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.SwitcherManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FCSPlayout.WPFApp
{
    internal class ChannelSwitcher: IChannelSwitcher
    {
        private Dictionary<Guid, BMDSwitcherInputInfo> _channelInputMap = 
            new Dictionary<Guid, BMDSwitcherInputInfo>();

        private ChannelInfo _defaultChannel;
        private IEnumerable<BMDSwitcherInfo> _switchers;
        private BMDSwitcherInfo _currentSwitcher=null;
        private SwitcherWrapper _switcherOperator=null;

        private static ChannelSwitcher _instance;
        private readonly IEnumerable<ChannelInfo> _channels;

        internal static ChannelSwitcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChannelSwitcher();
                }
                return _instance;
            }
        }

        public IEnumerable<ChannelInfo> Channels
        {
            get
            {
                return _channels;
            }
        }

        internal BMDSwitcherInputInfo GetInputInfo(ChannelInfo ch)
        {
            if (_channelInputMap.ContainsKey(ch.Id))
            {
                return _channelInputMap[ch.Id];
            }
            else
            {
                return null;
            }
        }

        private ChannelSwitcher()
        {
#if true
            _channels=PlayoutRepository.GetChannelInfos();
            this._defaultChannel = _channels.SingleOrDefault(i => i.Special);

            this._switchers = PlayoutRepository.GetBMDSwitcherInfos();
            if (_switchers.Any())
            {
                _currentSwitcher = _switchers.Single(i => i.Id == SettingsManager.BMDSwitcherId);
                IEnumerable<BMDSwitcherInputInfo> inputs =
                    PlayoutRepository.GetSwitcherInputInfos(SettingsManager.BMDSwitcherId, false);

                foreach (var channel in _channels)
                {
                    _channelInputMap.Add(channel.Id, inputs.SingleOrDefault(i => i.ChannelId == channel.Id));
                }

                _switcherOperator = FCSPlayout.SwitcherManagement.SwitcherWrapper.Get(_currentSwitcher.Address);
                _switcherOperator.CurrentProgramChanged += _switcherOperator_CurrentProgramChanged;
            }
            else
            {

            }
#endif
        }

        internal bool IsValid
        {
            get { return _switcherOperator != null; }
        }

        internal StringObjectPair<long> CurrentProgram
        {
            get { return _switcherOperator.CurrentProgram; }
        }

        public event EventHandler CurrentProgramChanged;

        private void _switcherOperator_CurrentProgramChanged(object sender, EventArgs e)
        {
            if (CurrentProgramChanged != null)
            {
                CurrentProgramChanged(this, EventArgs.Empty);
            }
        }

        public void SwitchChannelFor(IMediaSource mediaSource)
        {
#if true
            if (_switcherOperator != null)
            {
                ChannelInfo channel = GetChannel(mediaSource);
                Switch(channel);
            }
#endif
        }

        internal void Switch(string name)
        {
            if (_switcherOperator != null)
            {
                //var s = _channelInputMap[channel.Id];
                _switcherOperator.SetProgramInput(name/*s.Name*/);
            }
        }

        private void Switch(ChannelInfo channel)
        {
            if (_switcherOperator != null)
            {
                var s = _channelInputMap[channel.Id];
                _switcherOperator.SetProgramInput(s.Name);
            }
        }

        private ChannelInfo GetChannel(IMediaSource mediaSource)
        {
            var channelSource = mediaSource as IChannelMediaSource;
            if (channelSource != null)
            {
                return channelSource.Channel;
            }
            
            return this._defaultChannel;
        }
    }
}
