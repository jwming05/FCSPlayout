using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;

namespace FCSPlayout.PlaybillEditor
{
    public partial class PlaybillViewModel
    {
        private readonly DelegateCommand<IPlayableItem> _previewCommand;
        private readonly DelegateCommand _deletePlayItemCommand;
        private readonly DelegateCommand _editDurationCommand;
        private readonly DelegateCommand _changeStartTimeCommand;
        private readonly DelegateCommand _changeToAutoModeCommand;
        private readonly DelegateCommand _changeToTimingModeCommand;
        private readonly DelegateCommand _changeToBreakModeCommand;
        private readonly DelegateCommand _moveUpCommand;
        private readonly DelegateCommand _moveDownCommand;
        private readonly DelegateCommand _editCGItemsCommand;

        private readonly DelegateCommand _saveXmlCommand;
        private readonly DelegateCommand _openXmlCommand;
        private readonly DelegateCommand _clearCommand;

        private readonly DelegateCommand _savePlaybillCommand;
        private readonly DelegateCommand _loadPlaybillCommand;

        private readonly DelegateCommand _changeSourceCommand;
        private readonly DelegateCommand _changeSourceAndDurationCommand;

        private IEventAggregator _eventAggregator;
        private InteractionRequests _interactionRequests;
        private IPlayableItem _currentPreviewItem;

        private WrappedItemListAdapter<BindablePlayItem, IPlayItem> _listAdapter;

        private IPlayoutConfiguration _playoutConfig;
        private Playbill _playbill;
        private IUserService _userService;
        private MediaItem? _selectedMediaItem;

        private MediaItem? SelectedMediaItem
        {
            get { return _selectedMediaItem; }
            set
            {
                _selectedMediaItem = value;
                _changeSourceCommand.RaiseCanExecuteChanged();
                _changeSourceAndDurationCommand.RaiseCanExecuteChanged();
            }
        }

        public BindablePlayItem SelectedPlayItem
        {
            get { return _selectedPlayItem; }
            set
            {
                _selectedPlayItem = value;

                _deletePlayItemCommand.RaiseCanExecuteChanged();
                _editDurationCommand.RaiseCanExecuteChanged();
                _changeStartTimeCommand.RaiseCanExecuteChanged();
                _changeToAutoModeCommand.RaiseCanExecuteChanged();
                _changeToBreakModeCommand.RaiseCanExecuteChanged();
                _changeToTimingModeCommand.RaiseCanExecuteChanged();
                _moveUpCommand.RaiseCanExecuteChanged();
                _moveDownCommand.RaiseCanExecuteChanged();

                _changeSourceCommand.RaiseCanExecuteChanged();
                _changeSourceAndDurationCommand.RaiseCanExecuteChanged();
            }
        }

        #region Commands
        public ICommand PreviewCommand
        {
            get { return _previewCommand; }
        }

        public ICommand DeletePlayItemCommand
        {
            get { return _deletePlayItemCommand; }
        }

        public ICommand EditDurationCommand
        {
            get
            {
                return _editDurationCommand;
            }
        }

        public ICommand ChangeStartTimeCommand
        {
            get
            {
                return _changeStartTimeCommand;
            }
        }

        public ICommand ChangeToAutoModeCommand
        {
            get
            {
                return _changeToAutoModeCommand;
            }
        }

        public ICommand ChangeToBreakModeCommand
        {
            get
            {
                return _changeToBreakModeCommand;
            }
        }

        public ICommand ChangeToTimingModeCommand
        {
            get
            {
                return _changeToTimingModeCommand;
            }
        }

        public ICommand MoveUpCommand
        {
            get
            {
                return _moveUpCommand;
            }
        }

        public ICommand MoveDownCommand
        {
            get
            {
                return _moveDownCommand;
            }
        }

        public ICommand SaveXmlCommand
        {
            get
            {
                return _saveXmlCommand;
            }
        }

        public ICommand OpenXmlCommand
        {
            get
            {
                return _openXmlCommand;
            }
        }

        public ICommand SavePlaybillCommand
        {
            get
            {
                return _savePlaybillCommand;
            }
        }

        public ICommand LoadPlaybillCommand
        {
            get
            {
                return _loadPlaybillCommand;
            }
        }

        public ICommand ChangeSourceCommand
        {
            get
            {
                return _changeSourceCommand;
            }
        }

        public ICommand ChangeSourceAndDurationCommand
        {
            get
            {
                return _changeSourceAndDurationCommand;
            }
        }
        #endregion Commands

        private PlaybillService _playbillService=new PlaybillService();

        #region Command Methods
        private void Preview(IPlayableItem playableItem)
        {
            if (_eventAggregator != null && playableItem != null)
            {
                _currentPreviewItem = playableItem;
                _eventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Publish(playableItem);
            }
        }

        private bool CanDeletePlayItem()
        {
            return this.SelectedPlayItem != null && _playbillService.CanDelete(this.SelectedPlayItem.PlayItem);
        }

        private void DeletePlayItem()
        {
            if (CanDeletePlayItem())
            {
                var playItem = this.SelectedPlayItem.PlayItem;
                this.ConfirmationInteractionRequest.Raise(new Confirmation
                { Title = "删除确认", Content = "确实要删除播放项：'" + playItem.Title + "'吗？" },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            _playbillService.Delete(playItem);
                            DeletePlayItem(playItem);
                        }
                    });
            }
        }

        
        

        
        private bool CanEditDuration()
        {
            return this.SelectedPlayItem != null && 
                _playItemBehavior.CanEdit(this.SelectedPlayItem.PlayItem,EditOption.Duration);
        }

        private void EditDuration()
        {
            if (CanEditDuration())
            {
                EditDurationInteractionRequest.Raise(
                        new EditDurationConfirmation(new MediaItem(this.SelectedPlayItem.PlayItem.MediaSource, this.SelectedPlayItem.PlayItem.PlayRange))
                        { Title = this.SelectedPlayItem.Title },
                    n =>
                    {
                        if (n.Confirmed)
                        {

                            ChangeDuration(this.SelectedPlayItem.PlayItem, n.Duration);
                        }
                    });
            }
        }

        private bool CanChangeStartTime()
        {
            return this.SelectedPlayItem != null && _playItemBehavior.CanEdit(this.SelectedPlayItem.PlayItem, EditOption.StartTime);
        }

        private void ChangeStartTime()
        {
            if (CanChangeStartTime())
            {
                this.EditDateTimeInteractionRequest.Raise(
                    new EditDateTimeConfirmation { Time = this.SelectedPlayItem.StartTime, Title = "编辑开始时间" },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            ChangeStartTime(this.SelectedPlayItem.PlayItem, c.Time);
                        }
                    });
            }
        }

        private bool CanChangeSourceAndDuration()
        {
            return this.SelectedPlayItem != null && this.SelectedMediaItem!=null;
        }

        private void ChangeSourceAndDuration()
        {
            if (CanChangeSourceAndDuration())
            {
                var builder = CreateBuilder();
                try
                {
                    builder.ChangeSource(this.SelectedPlayItem.PlayItem, this.SelectedMediaItem.Value.Source,
                        this.SelectedMediaItem.Value.PlayRange);
                    Rebuild(builder);
                }
                catch (Exception ex)
                {
                    //editor.Rollback();
                    OnError(ex);
                }
            }
        }

        private bool CanChangeSource()
        {
            return this.SelectedPlayItem != null && this.SelectedMediaItem != null && 
                _playItemBehavior.CanEdit(this.SelectedPlayItem.PlayItem, EditOption.SourceOnly, this.SelectedMediaItem.Value.PlayRange);
        }

        private void ChangeSource()
        {
            if (CanChangeSource())
            {
                var builder = CreateBuilder();
                try
                {
                    var newRange = new PlayRange(this._selectedMediaItem.Value.PlayRange.StartPosition, 
                        this.SelectedPlayItem.PlayItem.CalculatedPlayDuration);
                    builder.ChangeSource(this.SelectedPlayItem.PlayItem, this.SelectedMediaItem.Value.Source, newRange);
                    Rebuild(builder);
                }
                catch (Exception ex)
                {
                    //editor.Rollback();
                    OnError(ex);
                }
            }
        }

        private bool CanChangeToAutoMode()
        {
            return this.SelectedPlayItem != null && 
                _playItemBehavior.CanEdit(this.SelectedPlayItem.PlayItem, EditOption.ScheduleMode, PlayScheduleMode.Auto);
            //this.SelectedPlayItem.ScheduleMode != PlayScheduleMode.Auto;
        }

        private void ChangeToAutoMode()
        {
            if (CanChangeToAutoMode())
            {
                var builder = CreateBuilder();
                try
                {
                    builder.ChangeToAuto(this.SelectedPlayItem.PlayItem);
                    Rebuild(builder);
                }
                catch (Exception ex)
                {
                    //editor.Rollback();
                    OnError(ex);
                }
            }
        }

        private bool CanChangeToBreakMode()
        {
            return this.SelectedPlayItem != null &&
                _playItemBehavior.CanEdit(this.SelectedPlayItem.PlayItem, EditOption.ScheduleMode, PlayScheduleMode.TimingBreak);
        }

        private void ChangeToBreakMode()
        {
            if (CanChangeToBreakMode())
            {
                this.EditDateTimeInteractionRequest.Raise(new EditDateTimeConfirmation { Time = this.SelectedPlayItem.StartTime, Title = "更改为定时插播" },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            var builder = CreateBuilder();
                            //using (var editor = this.Edit())
                            {
                                try
                                {
                                    builder.ChangeToTimingBreak(this.SelectedPlayItem.PlayItem);
                                    Rebuild(builder);
                                    //editor.ChangeSchedule(this.SelectedPlayItem.PlayItem, PlayScheduleMode.TimingBreak, c.Time);
                                }
                                catch (Exception ex)
                                {
                                    //editor.Rollback();
                                    OnError(ex);
                                }
                            }
                        }
                    });
            }
        }

        private bool CanChangeToTimingMode()
        {
            return this.SelectedPlayItem != null &&
                _playItemBehavior.CanEdit(this.SelectedPlayItem.PlayItem, EditOption.ScheduleMode, PlayScheduleMode.Timing);
        }

        private void ChangeToTimingMode()
        {
            if (CanChangeToTimingMode())
            {
                this.EditDateTimeInteractionRequest.Raise(new EditDateTimeConfirmation
                { Time = this.SelectedPlayItem.StartTime, Title = "更改为定时播" },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            var editor = CreateBuilder();
                            //using (var editor = this.Edit())
                            {
                                try
                                {
                                    editor.ChangeToTiming(this.SelectedPlayItem.PlayItem);
                                    Rebuild(editor);
                                    //editor.ChangeSchedule(this.SelectedPlayItem.PlayItem, PlayScheduleMode.Timing, c.Time);
                                }
                                catch (Exception ex)
                                {
                                    //editor.Rollback();
                                    OnError(ex);
                                }
                            }
                        }
                    });
            }
        }

        private bool CanMoveDown()
        {
            return this.SelectedPlayItem != null && _playItemBehavior.CanEdit(this.SelectedPlayItem.PlayItem, EditOption.Move);
                //!this.SelectedPlayItem.PlayItem.IsAutoPadding() && this.SelectedPlayItem.ScheduleMode==PlayScheduleMode.Auto;
        }

        private void MoveDown()
        {
            if (CanMoveDown())
            {
                var builder = CreateBuilder();
                //using (var editor = this.Edit())
                {
                    try
                    {
                        builder.MoveDown(this.SelectedPlayItem.PlayItem);
                        Rebuild(builder);
                    }
                    catch (Exception ex)
                    {
                        //editor.Rollback();
                        OnError(ex);
                    }
                }
            }
        }

        private bool CanMoveUp()
        {
            return this.SelectedPlayItem != null && _playItemBehavior.CanEdit(this.SelectedPlayItem.PlayItem, EditOption.Move);
            //!this.SelectedPlayItem.PlayItem.IsAutoPadding() && this.SelectedPlayItem.ScheduleMode == PlayScheduleMode.Auto;
        }

        private void MoveUp()
        {
            if (CanMoveUp())
            {
                var builder = CreateBuilder();
                //using (var editor = this.Edit())
                {
                    
                    try
                    {
                        builder.MoveUp(this.SelectedPlayItem.PlayItem);
                        Rebuild(builder);
                    }
                    catch (Exception ex)
                    {
                        //editor.Rollback();
                        OnError(ex);
                    }
                }
                
            }
        }

        

        private bool CanEditCGItems()
        {
            return this.SelectedPlayItem != null && 
                !this.SelectedPlayItem.PlayItem.IsAutoPadding() && (this.SelectedPlayItem.Source is IFileMediaSource);
        }

        private void EditCGItems()
        {
            if (CanEditCGItems())
            {
                this.EditCGItemsInteractionRequest.Raise(new EditCGItemsConfirmation
                {
                    Title = "编辑字幕",
                    Items = this.SelectedPlayItem.PlayItem.CGItems==null ? new CG.CGItemCollection() : this.SelectedPlayItem.PlayItem.CGItems.Clone()
                },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            this.SelectedPlayItem.PlayItem.CGItems = c.Items;

                            //using(var editor = this.Playlist.Edit())
                            //{
                            //    //editor
                            //}
                            //this.SelectedPlayItem.PlayItem.CGItems = c.Items.Clone();
                        }
                    });
            }
        }

        private void OpenXml()
        {
            // TODO: 提示修改保存。

            _interactionRequests.OpenFileInteractionRequest.Raise(
                new OpenFileDialogConfirmation() { Filter = "XML文件(*.xml)|*.xml", Multiselect = false },
                n =>
                {
                    if (n.Confirmed)
                    {
                        LoadFromXml(n.FileName);
                    }
                });
        }

        

        private bool CanSaveXml()
        {
            return _playItemCollection.Count > 0;
        }

        private void SaveXml()
        {
            if (CanSaveXml())
            {
                _interactionRequests.SaveFileInteractionRequest.Raise(new SaveFileDialogConfirmation()
                { Filter = "XML文件(*.xml)|*.xml", OverwritePrompt = true },
                n =>
                {
                    if (n.Confirmed)
                    {
                        SaveToXml(n.FileName, _playItemCollection.Select(i=>i.PlayItem));
                    }
                });
            }
        }

        private void SaveToXml(string fileName, IEnumerable<IPlayItem> playItems)
        {
            PlayItemXmlRepository.SaveToXml(fileName, playItems);
        }

        private void LoadFromXml(string fileName)
        {
            this.Clear();
            var newItems = PlayItemXmlRepository.LoadFromXml(fileName);
            for (int i = 0; i < newItems.Count; i++)
            {
                _playItemCollection.Add(_playItemCollection.CreateBindablePlayItem(newItems[i]));
            }
        }

        private bool CanSavePlaybill()
        {
            return _playItemCollection.Count>0; // this.Playbill.CanSave();
        }

        private void SavePlaybill()
        {
            if (CanSavePlaybill())
            {
                // TODO: 更新数据库之前提示？
                if (ValidateBeforeSave())
                {
                    if (_playbill == null)
                    {
                        _playbill = new Playbill();
                    }

                    _playbill.PlayItemCollection = _playItemCollection;
                    try
                    {
                        _playbill.Save(_playoutConfig.AutoPaddingMediaSource,_userService.CurrentUser);
                    }
                    catch(Exception ex)
                    {
                        OnError(ex);
                    }
                }
            }
        }

        private bool ValidateBeforeSave()
        {
            if (_playItemCollection.GetStopTime().Value < DateTime.Now.AddMinutes(5))
            {
                this.RaiseDisplayMessageInteractionRequest(
                        "错误",
                        "不能保存时间已经过期或即将过期的节目单。");
                return false;
            }

            for(int i = 0; i < _playItemCollection.Count; i++)
            {
                BindablePlayItem item = _playItemCollection[i];
                if(!item.Skipped() && item.PlayDuration < _playoutConfig.MinPlayDuration)
                {
                    this.RaiseDisplayMessageInteractionRequest(
                        "错误", 
                        string.Format("播放项{0}的时长太短，最低时长不能小于{1}，请修复错误后再保存。", item.Title, _playoutConfig.MinPlayDuration));
                    return false;
                }
            }

            //TODO: 是否需要一些其他提示？
            return true;
        }

        private bool CanLoadPlaybill()
        {
            return true;
        }

        private void LoadPlaybill()
        {
            if (CanLoadPlaybill())
            {
                // TODO: 提示保存

                this.LoadPlaybillInteractionRequest.Raise(new LoadPlaybillConfirmation { Title = "选择节目单", Playbills =LoadPlaybills() },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            BindablePlaybill bill = c.SelectedPlaybill;

                            if (bill != null)
                            {
                                List<IPlayItem> playItems = new List<IPlayItem>();
                                var playbill = Playbill.Load(bill.Id, playItems);

                                if (playItems.Count > 0)
                                {
                                    this.Clear();

                                    _playbill = playbill;

                                    foreach(var item in playItems)
                                    {
                                        _playItemCollection.Add(_playItemCollection.CreateBindablePlayItem(item));
                                    }
                                }
                            }
                        }
                    });
            }
        }

        private IEnumerable<BindablePlaybill> LoadPlaybills()
        {
            DateTime minStopTime = DateTime.Now.AddMinutes(5);
            return Playbill.LoadPlaybills(minStopTime).Select(i=> new BindablePlaybill(i)).ToArray();
        }
        #endregion Command Methods

        private PlayItemBehavior _playItemBehavior = new PlayItemBehavior();

        private NewPlaylistEditor Edit()
        {
            return new NewPlaylistEditor(_listAdapter, OnCommitted);
        }

        #region Interaction Requests
        public InteractionRequest<EditDurationConfirmation> EditDurationInteractionRequest
        {
            get { return _interactionRequests.EditDurationInteractionRequest; }
        }

        public InteractionRequest<Notification> DisplayMessageInteractionRequest
        {
            get { return _interactionRequests.DisplayMessageInteractionRequest; }
        }

        public InteractionRequest<Confirmation> ConfirmationInteractionRequest
        {
            get { return _interactionRequests.ConfirmationInteractionRequest; }
        }

        public InteractionRequest<EditDateTimeConfirmation> EditDateTimeInteractionRequest
        {
            get { return _interactionRequests.EditDateTimeInteractionRequest; }
        }

        public InteractionRequest<EditCGItemsConfirmation> EditCGItemsInteractionRequest
        {
            get { return _interactionRequests.EditCGItemsInteractionRequest; }
        }

        public InteractionRequest<OpenFileDialogConfirmation> OpenFileInteractionRequest
        {
            get { return _interactionRequests.OpenFileInteractionRequest; }
        }

        public InteractionRequest<SaveFileDialogConfirmation> SaveFileInteractionRequest
        {
            get { return _interactionRequests.SaveFileInteractionRequest; }
        }

        public InteractionRequest<LoadPlaybillConfirmation> LoadPlaybillInteractionRequest
        {
            get { return _interactionRequests.LoadPlaybillInteractionRequest; }
        }

        #endregion Interaction Requests

        private DateTime? _startTime;
        private DateTime? _stopTime;
        

        public DateTime? StartTime
        {
            get { return _startTime; }
            set
            {
                base.SetProperty(ref _startTime, value);
            }
        }

        public DateTime? StopTime
        {
            get { return _stopTime; }
            set
            {
                base.SetProperty(ref _stopTime, value);
            }
        }


        public TimeSpan? Duration
        {
            get
            {
                if(this.StartTime==null || this.StopTime == null)
                {
                    return null;
                }
                return this.StopTime.Value.Subtract(this.StartTime.Value);
            }
        }

        public IMediaFileImageResolver ImageResolver { get; private set; }

        public DelegateCommand ClearCommand
        {
            get
            {
                return _clearCommand;
            }
        }

        private void RaiseDisplayMessageInteractionRequest(string title, string message)
        {
            if (DisplayMessageInteractionRequest != null)
            {
                DisplayMessageInteractionRequest.Raise(new Notification { Title = title, Content = message });
            }
        }

        private void AddPlayItem(AddPlayItemPayload payload)
        {
            AddPlayItem(payload.MediaItem, payload.ScheduleMode, payload.StartTime);
        }

        private DateTime? maxStopTime=null;
        private void AddPlayItem(MediaItem mediaItem, PlayScheduleMode scheduleMode, DateTime? startTime)
        {
            PlaylistBuilder builder = CreateBuilder();
            PlaybillItem billItem = null;


            try
            {
                switch (scheduleMode)
                {
                    case PlayScheduleMode.Timing:
                        billItem = PlaybillItem.Timing(mediaItem, startTime.Value);
                        builder.AddTimingItem((TimingPlaybillItem)billItem);
                        break;
                    case PlayScheduleMode.Auto:
                        billItem = PlaybillItem.Auto(mediaItem);
                        builder.AddAutoItem(new AutoPlayItem(billItem),
                                this.SelectedPlayItem != null ? this.SelectedPlayItem.PlayItem : null);
                        break;
                    case PlayScheduleMode.TimingBreak:
                        billItem = PlaybillItem.TimingBreak(mediaItem, startTime.Value);
                        builder.AddTimingBreakItem((TimingPlaybillItem)billItem);
                        break;
                }

                Rebuild(builder);
            }
            catch (Exception ex)
            {
                //editor.Rollback();
                OnError(ex);
            }
        }

        private void Rebuild(PlaylistBuilder builder)
        {
            var items = builder.Build(maxStopTime);
            _listAdapter.Clear();
            foreach (var item in items)
            {
                _listAdapter.Add(item);
            }

            OnCommitted();
        }

        private PlaylistBuilder CreateBuilder()
        {
            return new PlaylistBuilder(_listAdapter);
        }

        private void ChangeDuration(IPlayItem playItem, TimeSpan newDuration)
        {
            var builder = CreateBuilder();
            
            try
            {
                builder.ChangePlayRange(playItem, new PlayRange(TimeSpan.Zero, newDuration));
                Rebuild(builder);
            }
            catch (Exception ex)
            {
                //editor.Rollback();
                OnError(ex);
            }
        }

        private void DeletePlayItem(IPlayItem playItem)
        {
            var builder = new PlaylistBuilder(_listAdapter);
            try
            {
                builder.RemoveItem(playItem);

                var items = builder.Build(maxStopTime);
                _listAdapter.Clear();
                foreach (var item in items)
                {
                    _listAdapter.Add(item);
                }

                OnCommitted();
            }
            catch (Exception ex)
            {
                //editor.Rollback();
                OnError(ex);
            }
        }

        private void Playlist_EditCompleted(object sender, EditCompletedEventArgs e)
        {
            //this.StartTime = this.Playlist.GetStartTime();
            //this.StopTime = this.Playlist.GetStopTime();
        }

        private void OnCommitted()
        {
            this.StartTime = this._playItemCollection.GetStartTime();
            this.StopTime = this._playItemCollection.GetStopTime();

            this.RaisePropertyChanged(nameof(this.Duration));

            _saveXmlCommand.RaiseCanExecuteChanged();
            _savePlaybillCommand.RaiseCanExecuteChanged();
            _clearCommand.RaiseCanExecuteChanged();
        }
        private void ChangeStartTime(IPlayItem playItem, DateTime time)
        {
            var builder = new PlaylistBuilder(_listAdapter);
            try
            {
                builder.ChangeStartTime(playItem, time);

                var items = builder.Build(maxStopTime);
                _listAdapter.Clear();
                foreach (var item in items)
                {
                    _listAdapter.Add(item);
                }

                OnCommitted();
            }
            catch (Exception ex)
            {
                //editor.Rollback();
                OnError(ex);
            }
        }

        private bool CanClear()
        {
            return _playItemCollection.Count > 0;
        }

        private void Clear()
        {
            // TODO: 提示保存。
            _playbill = null;
            _playItemCollection.Clear();
        }

        FCSPlayout.AppInfrastructure.IPlayableItemEditor IPlayableItemEditorFactory.CreateEditor()
        {
            return new PlayItemEditor(this.Edit(), OnError);
        }

        private void OnError(Exception ex)
        {
            RaiseDisplayMessageInteractionRequest("错误", ex.Message);
        }

        public PlayItemCollection PlayItemCollection
        {
            get { return _playItemCollection; }
        }

        class PlayItemEditor : FCSPlayout.AppInfrastructure.IPlayableItemEditor
        {
            private Action<Exception> _onError;
            private IPlaylistEditor _playlistEditor;

            public PlayItemEditor(IPlaylistEditor playlistEditor,Action<Exception> onError)
            {
                this._playlistEditor = playlistEditor;
                _onError = onError;
            }

            public void ChangePlayRange(IPlayableItem playableItem, PlayRange newRange)
            {
                ChangePlayRange(playableItem.PlayItem, newRange);
            }

            private void ChangePlayRange(IPlayItem playItem,PlayRange newRange)
            {
                try
                {
                    _playlistEditor.ChangePlayRange(playItem, newRange);
                }
                catch(Exception ex)
                {
                    ((NewPlaylistEditor)_playlistEditor).Rollback();
                    if (_onError != null)
                    {
                        _onError(ex);
                    }
                }
            }


            public void Dispose()
            {
                if (_playlistEditor != null)
                {
                    var temp = _playlistEditor;
                    _playlistEditor = null;
                    temp.Dispose();

                    _onError = null;
                }
            }
        }
    }

    enum EditOption
    {
        Duration,
        StartTime,
        SourceAndPlayRange,
        SourceOnly,
        ScheduleMode,
        Move,
    }

    class PlayItemBehavior
    {
        public bool CanEdit(IPlayItem playItem, EditOption option, object context = null)
        {
            switch (option)
            {
                case EditOption.Duration:
                    return !playItem.IsAutoPadding() && (playItem.MediaSource is IChannelMediaSource);
                case EditOption.StartTime:
                    return playItem.ScheduleMode == PlayScheduleMode.Timing || playItem.ScheduleMode == PlayScheduleMode.TimingBreak;
                case EditOption.SourceAndPlayRange:
                    return true;
                case EditOption.SourceOnly:
                    PlayRange newRange = (PlayRange)context;
                    return newRange.Duration >= playItem.CalculatedPlayDuration;
                case EditOption.ScheduleMode:
                    PlayScheduleMode newMode = (PlayScheduleMode)context;
                    return (newMode != playItem.ScheduleMode) && (newMode == PlayScheduleMode.Auto || !playItem.IsAutoPadding());
                case EditOption.Move:
                    return !playItem.IsAutoPadding() && playItem.ScheduleMode == PlayScheduleMode.Auto;
            }

            return false;
        }
    }
}
