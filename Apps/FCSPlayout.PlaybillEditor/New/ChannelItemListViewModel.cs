using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.PlaybillEditor
{
    public class ChannelItemListViewModel:BindableBase,Prism.IActiveAware
    {
        private ChannelMediaSource _selectedChannel;
        private PlayRange _playRange;

        public ChannelItemListViewModel(IRegionManager regionManager)
        {
            this.Channels = new List<ChannelMediaSource>(PlayoutRepository.GetChannelInfos(true).Select(i=>new ChannelMediaSource(i)));

            // TODO: 从配置中获取默认时长。
            _playRange = new PlayRange(TimeSpan.FromMinutes(30));

            _regionManager = regionManager;
        }

        public List<ChannelMediaSource> Channels { get; private set; }

        public ChannelMediaSource SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                _selectedChannel = value;

                if (_isActive && this.MediaItemSelector != null)
                {
                    this.MediaItemSelector.SelectedMediaItem =
                        this.SelectedChannel == null ? (MediaItem?)null : new MediaItem(this.SelectedChannel, _playRange);
                }

                this.RaisePropertyChanged(nameof(this.SelectedChannel));
            }
        }

        public TimeSpan Duration
        {
            get { return _playRange.Duration; }
            set
            {
                _playRange = new PlayRange(value);
                this.RaisePropertyChanged(nameof(this.Duration));
            }
        }

        public event EventHandler IsActiveChanged;

        private bool _isActive;
        private IRegionManager _regionManager;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (_isActive && this.MediaItemSelector != null)
                {
                    this.MediaItemSelector.SelectedMediaItem = this.SelectedChannel == null ? (MediaItem?)null : new MediaItem(this.SelectedChannel, new PlayRange(this.Duration));
                }
            }
        }

        public IMediaItemSelector MediaItemSelector
        {
            get { return _regionManager.Regions["mediaItemRegion"].Context as IMediaItemSelector; }
        }
    }
}
