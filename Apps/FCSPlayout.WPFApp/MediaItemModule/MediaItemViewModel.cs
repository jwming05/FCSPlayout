using System;
using FCSPlayout.Domain;
using Prism.Mvvm;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using System.Windows.Input;
using Prism.Events;
using FCSPlayout.AppInfrastructure;

namespace FCSPlayout.WPFApp
{
    public class MediaItemViewModel : BindableBase, IMediaItemSelector, IPlayScheduleInfoHost
    {
        private readonly DelegateCommand _addPlayItemCommand;
        private MediaItem? _selectedMediaItem;
        private PlayScheduleInfo _playScheduleInfo;
        private IEventAggregator _eventAggregator;

        public MediaItemViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _addPlayItemCommand = new DelegateCommand(AddPlayItem, CanAddPlayItem);
        }

        private bool CanAddPlayItem()
        {
            return this.SelectedMediaItem != null && this.PlayScheduleInfo != null;
        }

        private void AddPlayItem()
        {
            if (CanAddPlayItem())
            {
                _eventAggregator.GetEvent<PubSubEvent<AddPlayItemPayload>>().Publish(new AddPlayItemPayload
                {
                    MediaItem = SelectedMediaItem.Value,
                    ScheduleMode = PlayScheduleInfo.Mode,
                    StartTime = PlayScheduleInfo.StartTime
                });
            }
        }

        public MediaItem? SelectedMediaItem
        {
            get { return _selectedMediaItem; }
            set
            {
                _selectedMediaItem = value;
                _addPlayItemCommand.RaiseCanExecuteChanged();
            }
        }

        public PlayScheduleInfo PlayScheduleInfo
        {
            get { return _playScheduleInfo; }
            set
            {
                _playScheduleInfo = value;
                _addPlayItemCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddPlayItemCommand
        {
            get
            {
                return _addPlayItemCommand;
            }
        }
    }
}