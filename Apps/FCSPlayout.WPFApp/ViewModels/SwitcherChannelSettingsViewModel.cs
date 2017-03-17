using FCSPlayout.Entities;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FCSPlayout.WPFApp.ViewModels
{
    public class SwitcherChannelSettingsViewModel:BindableBase
    {
        private ChannelInfo _selectedChannelInfo;
        private BMDSwitcherInputInfo _selectedSwitcherInputInfo;
        private ChannelInfo _nullInfo;
        private Guid _switcherId = Guid.Empty; // int.MinValue;

        public SwitcherChannelSettingsViewModel()
        {
            this.ChannelInfos = new ObservableCollection<ChannelInfo>(PlayoutRepository.GetChannelInfos());
            this.SwitcherInputInfos = new ObservableCollection<BMDSwitcherInputInfo>();
            _nullInfo = new ChannelInfo { Title="未设置" };
            this.ChannelInfos.Add(_nullInfo);
        }

        internal void Initialize(Guid switcherId)
        {
            _switcherId = switcherId;
            foreach (var item in PlayoutRepository.GetSwitcherInputInfos(switcherId))
            {
                this.SwitcherInputInfos.Add(item);
            }
        }

        public ObservableCollection<ChannelInfo> ChannelInfos { get; private set; }

        public ObservableCollection<BMDSwitcherInputInfo> SwitcherInputInfos { get; private set; }

        public ChannelInfo SelectedChannelInfo
        {
            get { return _selectedChannelInfo; }
            set
            {
                if (_selectedChannelInfo != value)
                {
                    _selectedChannelInfo = value;
                    if(_selectedChannelInfo!=null && this.SelectedSwitcherInputInfo != null)
                    {
                        this.SelectedSwitcherInputInfo.ChannelId = _selectedChannelInfo != _nullInfo ? _selectedChannelInfo.Id : (System.Guid?)null;
                    }

                    OnPropertyChanged("SelectedChannelInfo");
                }
            }
        }

        public BMDSwitcherInputInfo SelectedSwitcherInputInfo
        {
            get { return _selectedSwitcherInputInfo; }
            set
            {
                if (_selectedSwitcherInputInfo != value)
                {
                    _selectedSwitcherInputInfo = value;
                    if(_selectedSwitcherInputInfo!=null)
                    {
                        if (_selectedSwitcherInputInfo.ChannelId!=null)
                        {
                            this.SelectedChannelInfo = this.ChannelInfos.Single(c=>c.Id==_selectedSwitcherInputInfo.ChannelId);
                        }
                        else
                        {
                            this.SelectedChannelInfo = _nullInfo;
                        }
                    }

                    OnPropertyChanged("SelectedSwitcherInputInfo");
                }
            }
        }

        internal void Save()
        {
            PlayoutRepository.Save(_switcherId, (IEnumerable<BMDSwitcherInputInfo>)this.SwitcherInputInfos);
        }
    }
}
