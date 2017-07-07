using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.PlaybillEditor;
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

namespace FCSPlayout.PlaybillEditor
{
    public partial class PlaybillViewModel : ViewModelBase,IPlayableItemEditorFactory
    {
        private Playlist _playlist;
        private PlayItemCollection _playItemCollection;
        private BindablePlayItem _selectedPlayItem;            
        //private readonly DelegateCommand _createPlaybillCommand;

        public PlaybillViewModel(IEventAggregator eventAggregator, 
            IMediaFileImageResolver imageResolver,IMediaFilePathResolver filePathResolver, InteractionRequests interactionRequests,
            IPlayoutConfiguration playoutConfig,IUserService userService)
        {
            _playoutConfig = playoutConfig;
            _interactionRequests = interactionRequests;
            _userService = userService;
            _playItemCollection = new PlayItemCollection(filePathResolver,this);
            
            this.ImageResolver = imageResolver;

            _savePlaybillCommand = new DelegateCommand(SavePlaybill, CanSavePlaybill);
            _loadPlaybillCommand = new DelegateCommand(LoadPlaybill, CanLoadPlaybill);

            this.Playlist = new Playlist(_playItemCollection);
            this.Playlist.EditCompleted += Playlist_EditCompleted;

            _listAdapter = new WrappedItemListAdapter<BindablePlayItem, IPlayItem>(_playItemCollection, 
                (i) => _playItemCollection.CreateBindablePlayItem(i));


            _deletePlayItemCommand = new DelegateCommand(DeletePlayItem, CanDeletePlayItem);

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

            _changeSourceCommand = new DelegateCommand(ChangeSource, CanChangeSource);
            _changeSourceAndDurationCommand = new DelegateCommand(ChangeSourceAndDuration, CanChangeSourceAndDuration);

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PubSubEvent<AddPlayItemPayload>>().Subscribe(AddPlayItem);

            _eventAggregator.GetEvent<PubSubEvent<MediaItem?>>().Subscribe((i) => this.SelectedMediaItem = i);
        }
        
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
                //var strPath = ((FileMediaSource)playItem.MediaItem.Source).FileName;

                //EditMediaItemInteractionRequest.Raise(new EditMediaItemConfirmation(strPath, playItem.MediaItem.PlayRange)
                //{ Title = playItem.Title },
                //n =>
                //{
                //    if (n.Confirmed)
                //    {
                //        var newPlayRange = n.PlayRange;
                //        //var oldPlayRange = playItem.PlayItem.PlaybillItem.MediaItem.PlayRange;
                //        if (newPlayRange == oldPlayRange)
                //        {
                //            return;
                //        }

                //        try
                //        {
                //            //this.Playbill.SetPlayRange(playItem.PlayItem, newPlayRange);
                //            //playItem.NotifyPlayRangeChanged();
                //        }
                //        catch (Exception ex)
                //        {
                //        }
                //    }
                //});
            }
        }
    }
}
