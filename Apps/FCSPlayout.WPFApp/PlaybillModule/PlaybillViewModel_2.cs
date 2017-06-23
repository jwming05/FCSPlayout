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
using System.Diagnostics;

namespace FCSPlayout.WPFApp
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
        private readonly DelegateCommand _forcePlayCommand;

        private readonly DelegateCommand _loadFirstPlaybillCommand;

        private IEventAggregator _eventAggregator;
        private InteractionRequests _interactionRequests;
        private IPlayableItem _currentPreviewItem;

        private WrappedItemListAdapter<BindablePlayItem, IPlayItem> _listAdapter;

        private IPlayoutConfiguration _playoutConfig;
        private Playbill _playbill;
        private BindablePlaybill _bindablePlaybill;

        private IUserService _userService;

        private readonly DelegateCommand _changeSourceCommand;
        private readonly DelegateCommand _changeSourceAndDurationCommand;

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

                _forcePlayCommand.RaiseCanExecuteChanged();
            }
        }

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

        #region Command Methods
        private bool CanChangeSourceAndDuration()
        {
            return this.SelectedPlayItem != null && this.SelectedMediaItem != null;
        }

        private void ChangeSourceAndDuration()
        {
            if (CanChangeSourceAndDuration())
            {
                using (var editor = this.Edit())
                {
                    try
                    {
                        editor.ChangeMediaSource(this.SelectedPlayItem.PlayItem, this.SelectedMediaItem.Value.Source,
                            this.SelectedMediaItem.Value.PlayRange);
                    }
                    catch (Exception ex)
                    {
                        editor.Rollback();
                        OnError(ex);
                    }
                }
            }
        }

        private bool CanChangeSource()
        {
            return this.SelectedPlayItem != null && this.SelectedMediaItem != null
                && this.SelectedMediaItem.Value.Duration >= this.SelectedPlayItem.PlayDuration;
        }

        private void ChangeSource()
        {
            if (CanChangeSource())
            {
                using (var editor = this.Edit())
                {
                    try
                    {
                        editor.ChangeMediaSource(this.SelectedPlayItem.PlayItem, this.SelectedMediaItem.Value.Source);
                    }
                    catch (Exception ex)
                    {
                        editor.Rollback();
                        OnError(ex);
                    }
                }
            }
        }
        private void Preview(IPlayableItem playableItem)
        {
            if (/*_eventAggregator != null && */playableItem != null)
            {
                _currentPreviewItem = playableItem;
                this.PreviewInteractionRequest.Raise(new PreviewRequestConfirmation(playableItem) { Title = "预览",PlayItemEditorFactory=this },
                (c) =>
                {
                    if (c.Confirmed)
                    {
                    }

                    playableItem.ClosePreview();
                });

                //_eventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Publish(playableItem);
            }       
        }

        private bool CanDeletePlayItem()
        {
            return this.SelectedPlayItem != null; // && this.Playlist.CanDelete(this.SelectedPlayItem.PlayItem);
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
                            DeletePlayItem(playItem);
                        }
                    });
            }
        }

        private bool CanEditDuration()
        {
            return this.SelectedPlayItem != null && !this.SelectedPlayItem.PlayItem.IsAutoPadding() && 
                (this.SelectedPlayItem.Source is IChannelMediaSource);
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
            return this.SelectedPlayItem != null && 
                (this.SelectedPlayItem.ScheduleMode==PlayScheduleMode.Timing || this.SelectedPlayItem.ScheduleMode == PlayScheduleMode.TimingBreak);
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

        private bool CanChangeToAutoMode()
        {
            return this.SelectedPlayItem != null && this.SelectedPlayItem.ScheduleMode != PlayScheduleMode.Auto;
        }

        private void ChangeToAutoMode()
        {
            if (CanChangeToAutoMode())
            {
                using (var editor = this.Edit())
                {
                    try
                    {
                        editor.ChangeSchedule(this.SelectedPlayItem.PlayItem, PlayScheduleMode.Auto, null);
                    }
                    catch (Exception ex)
                    {
                        editor.Rollback();
                        OnError(ex);
                    }
                }
            }
        }

        private bool CanChangeToBreakMode()
        {
            return this.SelectedPlayItem != null && !this.SelectedPlayItem.PlayItem.IsAutoPadding() && this.SelectedPlayItem.ScheduleMode!=PlayScheduleMode.TimingBreak;
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
                            using (var editor = this.Edit())
                            {
                                try
                                {
                                    editor.ChangeSchedule(this.SelectedPlayItem.PlayItem, PlayScheduleMode.TimingBreak, c.Time);
                                }
                                catch (Exception ex)
                                {
                                    editor.Rollback();
                                    OnError(ex);
                                }
                            }
                        }
                    });
            }
        }

        private bool CanChangeToTimingMode()
        {
            return this.SelectedPlayItem != null && !this.SelectedPlayItem.PlayItem.IsAutoPadding() && this.SelectedPlayItem.ScheduleMode != PlayScheduleMode.Timing;
        }

        private void ChangeToTimingMode()
        {
            if (CanChangeToTimingMode())
            {
                this.EditDateTimeInteractionRequest.Raise(new EditDateTimeConfirmation { Time = this.SelectedPlayItem.StartTime, Title = "更改为定时播" },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            using (var editor = this.Edit())
                            {
                                try
                                {
                                    editor.ChangeSchedule(this.SelectedPlayItem.PlayItem, PlayScheduleMode.Timing, c.Time);
                                }
                                catch (Exception ex)
                                {
                                    editor.Rollback();
                                    OnError(ex);
                                }
                            }
                        }
                    });
            }
        }

        private bool CanMoveDown()
        {
            return this.SelectedPlayItem != null && !this.SelectedPlayItem.PlayItem.IsAutoPadding() && this.SelectedPlayItem.ScheduleMode==PlayScheduleMode.Auto;
        }

        private void MoveDown()
        {
            if (CanMoveDown())
            {
                using (var editor = this.Edit())
                {
                    try
                    {
                        editor.MoveDown(this.SelectedPlayItem.PlayItem);
                    }
                    catch (Exception ex)
                    {
                        editor.Rollback();
                        OnError(ex);
                    }
                }
            }
        }

        private bool CanMoveUp()
        {
            return this.SelectedPlayItem != null && !this.SelectedPlayItem.PlayItem.IsAutoPadding() && this.SelectedPlayItem.ScheduleMode == PlayScheduleMode.Auto;
        }

        private void MoveUp()
        {
            if (CanMoveUp())
            {
                using (var editor = this.Edit())
                {
                    
                    try
                    {
                        editor.MoveUp(this.SelectedPlayItem.PlayItem);
                    }
                    catch (Exception ex)
                    {
                        editor.Rollback();
                        OnError(ex);
                    }
                }
                
            }
        }

        

        private bool CanEditCGItems()
        {
            return this.SelectedPlayItem != null && !this.SelectedPlayItem.PlayItem.IsAutoPadding() && (this.SelectedPlayItem.Source is IFileMediaSource);
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

        private bool CanLoadFirstPlaybill()
        {
            return true;
        }

        private void LoadFirstPlaybill()
        {
            var bill = this.LoadPlaybills().FirstOrDefault();
            if (bill != null)
            {
                List<IPlayItem> playItems = new List<IPlayItem>();
                var playbill = Playbill.Load(bill.Id, playItems);

                if (playItems.Count > 0)
                {
                    //this.Clear();

                    _playbill = playbill;

                    _bindablePlaybill = bill;

                    foreach (var item in playItems)
                    {
                        _playItemCollection.Add(_playItemCollection.CreateBindablePlayItem(item));
                    }
                }
            }
        }

        private bool CanLoadPlaybill()
        {
            return true;
        }

        private void LoadPlaybill()
        {
            if (CanLoadPlaybill())
            {
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

                                if (playItems.Count == 0) return;

                                if (_playItemCollection.Count > 0)
                                {

                                    var maxStopTime = playItems[0].StartTime;
                                    if (maxStopTime > _playItemCollection.Last().StopTime)
                                    {
                                        int conflictCount = 0;
                                        // TODO: 提示截短。

                                    for(int i = _playItemCollection.Count - 1; i >= 0; i--)
                                        {
                                            var temp = _playItemCollection[i];
                                            if (temp.StopTime <= maxStopTime)
                                            {
                                                break;
                                            }

                                            if (temp.ScheduleMode != PlayScheduleMode.Auto)
                                            {
                                                conflictCount++;
                                            }
                                        }

                                        if (conflictCount > 0)
                                        {
                                            this.RaiseDisplayMessageInteractionRequest("错误", "新加载的节目单的开始时间为" + maxStopTime.ToString("yyyy-MM--dd HH:mm:ss") + "，在这个时间段内与现有定时播或定时插播存在时间冲突。");
                                            return;
                                        }
                                    }
                                }
                                
                                //this.Clear();

                                _playbill = playbill;
                                _bindablePlaybill = bill;

                                _playItemCollection.Append(playItems);
                                //foreach (var item in playItems)
                                //{
                                //    _playItemCollection.Add(_playItemCollection.CreateBindablePlayItem(item));
                                //}
                            }
                        }
                    });
            }
        }

        private bool CanForcePlay()
        {
            return this.SelectedPlayItem != null && this.SelectedPlayItem.ScheduleMode==PlayScheduleMode.Auto 
                && _playItemCollection.CanForcePlay(this.SelectedPlayItem);
        }

        private void ForcePlay()
        {
            if (this.CanForcePlay())
            {
                using(var editor = this.Edit())
                {
                    editor.ForcePlay(_playItemCollection.CurrentItem, this.SelectedPlayItem.PlayItem);
                }
                //_playItemCollection.ForcePlay(this.SelectedPlayItem);
            }
        }

        private IEnumerable<BindablePlaybill> LoadPlaybills()
        {
            DateTime minStopTime = DateTime.Now.AddMinutes(5);
            if (_bindablePlaybill == null)
            {
                return Playbill.LoadPlaybills(minStopTime).Select(i => new BindablePlaybill(i)).ToArray();
            }
            else
            {
                return Playbill.LoadPlaybills(minStopTime,_bindablePlaybill.StartTime).Select(i => new BindablePlaybill(i)).ToArray();
            }
        }
        #endregion Command Methods

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

        private void AddPlayItem(MediaItem mediaItem, PlayScheduleMode scheduleMode, DateTime? startTime)
        {
            using (var editor = this.Edit())
            {
                try
                {

                    PlaybillItem billItem = null;
                    switch (scheduleMode)
                    {
                        case PlayScheduleMode.Timing:
                            billItem = PlaybillItem.Timing(mediaItem, startTime.Value);
                            editor.AddTiming((TimingPlaybillItem)billItem);
                            break;
                        case PlayScheduleMode.Auto:
                            billItem = PlaybillItem.Auto(mediaItem);
                            if (this.SelectedPlayItem != null)
                            {
                                editor.AddAuto(this.SelectedPlayItem.PlayItem, new AutoPlayItem(billItem));
                            }
                            else
                            {
                                editor.AddAuto(new AutoPlayItem(billItem));
                            }
                            break;
                        case PlayScheduleMode.TimingBreak:
                            billItem = PlaybillItem.TimingBreak(mediaItem, startTime.Value);
                            editor.AddTiming((TimingPlaybillItem)billItem);
                            break;
                    }

                }
                catch (Exception ex)
                {
                    editor.Rollback();
                    RaiseDisplayMessageInteractionRequest("错误", ex.Message);
                }
            }
        }

        private void ChangeDuration(IPlayItem playItem, TimeSpan newDuration)
        {
            using (var editor = this.Edit())
            {
                try
                {
                    editor.ChangePlayRange(playItem, new PlayRange(TimeSpan.Zero, newDuration));
                }
                catch (Exception ex)
                {
                    editor.Rollback();
                    OnError(ex);
                }
            }
        }

        private void DeletePlayItem(IPlayItem playItem)
        {
            using (var editor = this.Edit())
            {
                
                try
                {
                    editor.Delete(playItem);
                }
                catch (Exception ex)
                {
                    editor.Rollback();
                    OnError(ex);
                }
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

            this._playItemCollection.OnCommitted();
            _saveXmlCommand.RaiseCanExecuteChanged();
            _savePlaybillCommand.RaiseCanExecuteChanged();
            _clearCommand.RaiseCanExecuteChanged();

            // sync message
            SendPlaylist();
        }

        private void SendPlaylist()
        {
            // TODO: send playlist.

            //PlayControlService.Current.SendPlaylistRequest();
            
            var list = new List<IPlayItem>();
            for(int i = 0; i < _playItemCollection.Count; i++)
            {
                var item=_playItemCollection[i];
                list.Add(item.PlayItem);
            }

            var count = list.Count;
            Trace.WriteLine($"将发送{list.Count}项。");

        }

        private void ReceivePlaylist()
        {
            // TODO: update playlist when received.
        }

        private void ChangeStartTime(IPlayItem playItem, DateTime time)
        {
            using (var editor = this.Edit())
            {
                
                try
                {
                    editor.ChangeStartTime(playItem, time);
                }
                catch (Exception ex)
                {
                    editor.Rollback();
                    OnError(ex);
                }
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
            return new PlayableItemEditor(this.Edit(), OnError);
        }

        private void OnError(Exception ex)
        {
            RaiseDisplayMessageInteractionRequest("错误", ex.Message);
        }

        public InteractionRequest<PreviewRequestConfirmation> PreviewInteractionRequest
        {
            get { return _interactionRequests.PreviewInteractionRequest; }
        }

        public DelegateCommand ForcePlayCommand
        {
            get
            {
                return _forcePlayCommand;
            }
        }

        public DelegateCommand LoadFirstPlaybillCommand
        {
            get
            {
                return _loadFirstPlaybillCommand;
            }
        }

        class PlayableItemEditor : FCSPlayout.AppInfrastructure.IPlayableItemEditor
        {
            private Action<Exception> _onError;
            private IPlaylistEditor _playlistEditor;

            public PlayableItemEditor(IPlaylistEditor playlistEditor,Action<Exception> onError)
            {
                this._playlistEditor = playlistEditor;
                _onError = onError;
            }

            public void ChangePlayRange(IPlayableItem playItem, PlayRange newRange)
            {
                this.ChangePlayRange(playItem.PlayItem, newRange);
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
}
