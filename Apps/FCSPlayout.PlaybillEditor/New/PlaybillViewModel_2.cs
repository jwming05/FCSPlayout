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


        private IEventAggregator _eventAggregator;
        private InteractionRequests _interactionRequests;
        private IPlayableItem _currentPreviewItem;

        private WrappedItemListAdapter<BindablePlayItem, IPlayItem> _listAdapter;

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
        #endregion Commands

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
                        //foreach (var item in LoadFromXml(n.FileName))
                        //{
                        //    if (!string.IsNullOrEmpty(item.FilePath) && System.IO.File.Exists(item.FilePath))
                        //    {
                        //        _mediaItemCollection.Add(item);
                        //    }

                        //}
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
            PlayItemXmlRepository.SaveToXml(fileName, playItems, _mediaSourceConverter);
        }

        private void LoadFromXml(string fileName)
        {
            throw new NotImplementedException();
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

            _saveXmlCommand.RaiseCanExecuteChanged();
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

        IPlayItemEditor IPlayItemEditorFactory.CreateEditor()
        {
            return new PlayItemEditor(this.Edit(), OnError);
        }

        private void OnError(Exception ex)
        {
            RaiseDisplayMessageInteractionRequest("错误", ex.Message);
        }
        class PlayItemEditor : IPlayItemEditor
        {
            private Action<Exception> _onError;
            private IPlaylistEditor _playlistEditor;

            public PlayItemEditor(IPlaylistEditor playlistEditor,Action<Exception> onError)
            {
                this._playlistEditor = playlistEditor;
                _onError = onError;
            }

            public void ChangePlayRange(IPlayItem playItem,PlayRange newRange)
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
