using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FCSPlayout.WPFApp
{
    public partial class PlaybillViewModel : ViewModelBase,IPlayableItemEditorFactory
    {
        private Playlist _playlist;
        private PlayItemCollection _playItemCollection;
        
        private BindablePlayItem _selectedPlayItem;        
        
        private readonly DelegateCommand _createPlaybillCommand;

        public PlaybillViewModel(IEventAggregator eventAggregator, 
            IMediaFileImageResolver imageResolver, PlayItemCollection playItemCollection, 
            InteractionRequests interactionRequests,
            IPlayoutConfiguration playoutConfig,IUserService userService)
        {
            _playoutConfig = playoutConfig;
            _interactionRequests = interactionRequests;
            _userService = userService;
            _playItemCollection = playItemCollection; 
            
            this.ImageResolver = imageResolver;

            _savePlaybillCommand = new DelegateCommand(SavePlaybill, CanSavePlaybill);
            _loadPlaybillCommand = new DelegateCommand(LoadPlaybill, CanLoadPlaybill);
            _loadFirstPlaybillCommand= new DelegateCommand(LoadFirstPlaybill, CanLoadFirstPlaybill);

            //Playbill = new Playbill();
            this.Playlist = new Playlist(_playItemCollection);
            this.Playlist.EditCompleted += Playlist_EditCompleted;

            _listAdapter = new WrappedItemListAdapter<BindablePlayItem, IPlayItem>(_playItemCollection, (i) => _playItemCollection.CreateBindablePlayItem(i));

            _deletePlayItemCommand = new DelegateCommand(DeletePlayItem, CanDeletePlayItem);

            //_editMediaItemCommand = new DelegateCommand<object>(EditMediaItem, CanEditMediaItem);
            _editDurationCommand = new DelegateCommand(EditDuration, CanEditDuration);
            _changeStartTimeCommand = new DelegateCommand(ChangeStartTime, CanChangeStartTime);

            _changeToAutoModeCommand = new DelegateCommand(ChangeToAutoMode, CanChangeToAutoMode);
            _changeToBreakModeCommand = new DelegateCommand(ChangeToBreakMode, CanChangeToBreakMode);
            _changeToTimingModeCommand = new DelegateCommand(ChangeToTimingMode, CanChangeToTimingMode);

            _moveUpCommand = new DelegateCommand(MoveUp, CanMoveUp);
            _moveDownCommand = new DelegateCommand(MoveDown, CanMoveDown);

            //
            _saveXmlCommand = new DelegateCommand(SaveXml, CanSaveXml);
            _openXmlCommand = new DelegateCommand(OpenXml);

            _clearCommand = new DelegateCommand(Clear,CanClear);
            _editCGItemsCommand= new DelegateCommand(EditCGItems, CanEditCGItems);

            _previewCommand = new DelegateCommand<IPlayableItem>(Preview);

            _forcePlayCommand = new DelegateCommand(ForcePlay, CanForcePlay);
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PubSubEvent<AddPlayItemPayload>>().Subscribe(AddPlayItem);

            _changeSourceCommand = new DelegateCommand(ChangeSource, CanChangeSource);

            _eventAggregator.GetEvent<PubSubEvent<MediaItem?>>().Subscribe((i) => this.SelectedMediaItem = i);
            _changeSourceAndDurationCommand = new DelegateCommand(ChangeSourceAndDuration, CanChangeSourceAndDuration);

            
        }

        

        //internal void ChangeSource(BindablePlayItem playItem, MediaItem mediaItem)
        //{
        //}

        //internal bool CanChangeSource(object item)
        //{
        //    BindablePlayItem playItem = item as BindablePlayItem;
        //    return playItem != null; 
        //}
        
        private Playlist Playlist
        {
            get
            {
                return _playlist;
            }

            set
            {
                if (_playlist != value)
                {
                    _playlist = value;
                }
            }
        }

        public PlayItemCollection PlayItemCollection
        {
            get { return _playItemCollection; }
        }

                

        public ICommand EditCGItemsCommand
        {
            get
            {
                return _editCGItemsCommand;
            }
        }
        

        internal void UpdateMenuCommands()
        {
            _editDurationCommand.RaiseCanExecuteChanged();
            _changeStartTimeCommand.RaiseCanExecuteChanged();
        }


        private bool CanEditMediaItem()
        {
            return this.SelectedPlayItem != null && !this.SelectedPlayItem.PlayItem.IsAutoPadding() && (this.SelectedPlayItem.Source is IFileMediaSource);
        }

        private void EditMediaItem()
        {
            if (CanEditMediaItem())
            {
            }
        }
        #region
        

        public DelegateCommand CreatePlaybillCommand
        {
            get
            {
                return _createPlaybillCommand;
            }
        }

        private bool CanCreatePlaybill()
        {
            return true;
        }
        #endregion
    }
}
