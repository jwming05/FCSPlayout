using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using FCSPlayout.WPFApp.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace FCSPlayout.WPFApp.ViewModels
{
    public class PlaybillViewModel : ViewModelBase
    {
        private PlayItemCollection2 _playItemCollection;
        //private Playbill _playbill;
        //private List<IPlayItem> _playItems = new List<IPlayItem>();

        //private Playbill _playbill;
        private Playlist _playlist; // = new Playlist();

        private readonly DelegateCommand _addPlayItemCommand;
        private readonly DelegateCommand _deletePlayItemCommand;
        private BindableMediaItem _selectedMediaItem;
        private BindablePlayItem _selectedPlayItem;
        private FCSPlayout.WPF.Core.PlayScheduleInfo _playScheduleInfo;

        private readonly DelegateCommand _forcePlayCommand;
        private readonly InteractionRequest<Notification> _forcePlayRequest;
        private readonly DelegateCommand<object> _editMediaItemCommand;
        private readonly DelegateCommand<object> _editDurationCommand;

        private readonly DelegateCommand<object> _moveUpCommand;
        private readonly DelegateCommand<object> _moveDownCommand;
        private readonly DelegateCommand<object> _changeToAutoModeCommand;
        private readonly DelegateCommand<object> _changeToBreakModeCommand;

        private readonly DelegateCommand<object> _changeToTimingModeCommand;
        private readonly DelegateCommand<object> _changeStartTimeCommand;

        private readonly DelegateCommand<object> _editCGItemsCommand;

        private readonly DelegateCommand _savePlaybillCommand;
        private readonly DelegateCommand _loadPlaybillCommand;

        //private PlayItemList _playItemList;
        //private ObservableCollection<BindablePlayItem> _bindablePlayItems;
        public PlaybillViewModel()
        {
            //var temp = new ObservablePlayItemCollection();
            //_playItemCollection = temp;
            //_playItemsView = new CollectionView(temp);

            _savePlaybillCommand = new DelegateCommand(SavePlaybill, CanSavePlaybill);
            _loadPlaybillCommand = new DelegateCommand(LoadPlaybill, CanLoadPlaybill);

            //_bindablePlayItems = new ObservableCollection<BindablePlayItem>();
            //_bindablePlayItems.CollectionChanged += BindablePlayItems_CollectionChanged;
            _playItemCollection = new PlayItemCollection2(PlayControlService.Current);
            this.Playlist = new Playlist2(_playItemCollection);

            //_playItemList = new WPFApp.PlayItemList(_bindablePlayItems);
            //Playbill = new Playbill(_playItemList);

            //this.Playbill = new Playbill(_playItemCollection);
            //var collectionAdapter = new PlayItemCollectionAdapter2(_playItemCollection, (p) => new BindablePlayItem(p));
            //this.Playbill = new Playbill(collectionAdapter);

            //this.Playbill = new Playbill(_playItems);
            //this.Playbill.AfterExecuteAction += Playbill_AfterExecuteAction;


            //this.Playbill.PlayItemsChanged += Playbill_PlayItemsChanged;

            _addPlayItemCommand = new DelegateCommand(AddPlayItem, CanAddPlayItem);

            _deletePlayItemCommand = new DelegateCommand(DeletePlayItem, CanDeletePlayItem);
            _forcePlayCommand = new DelegateCommand(ForcePlay,CanForcePlay);
            _forcePlayRequest = new InteractionRequest<Notification>();

            _editMediaItemCommand = new DelegateCommand<object>(EditMediaItem, CanEditMediaItem);
            _editDurationCommand = new DelegateCommand<object>(EditDuration, CanEditDuration);

            _moveUpCommand = new DelegateCommand<object>(MoveUp, CanMoveUp);
            _moveDownCommand = new DelegateCommand<object>(MoveDown, CanMoveDown);

            _changeToAutoModeCommand= new DelegateCommand<object>(ChangeToAutoMode, CanChangeToAutoMode);
            _changeToBreakModeCommand= new DelegateCommand<object>(ChangeToBreakMode, CanChangeToBreakMode);
            _changeToTimingModeCommand = new DelegateCommand<object>(ChangeToTimingMode, CanChangeToTimingMode);
            _changeStartTimeCommand = new DelegateCommand<object>(ChangeStartTime, CanChangeStartTime);
            _editCGItemsCommand= new DelegateCommand<object>(EditCGItems, CanEditCGItems);
        }

        private void BindablePlayItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _playlist.Clear();
            //for(int i = 0; i < _bindablePlayItems.Count; i++)
            {
                //_playlist.Add(_bindablePlayItems[i]);
            }
            //_playlist.RaiseCollectionChanged();
        }



        //private void Playbill_AfterExecuteAction(object sender, AfterExecuteActionEventArgs e)
        //{
        //    this._playItemCollection.Clear();

        //    for(int i = 0; i < _playItems.Count; i++)
        //    {
        //        _playItemCollection.Add(new BindablePlayItem(_playItems[i]));
        //    }

        //    _playItems.RaiseCollectionChanged();
        //}

        public ICommand EditMediaItemCommand
        {
            get
            {
                return _editMediaItemCommand;
            }
        }

        public ICommand EditDurationCommand
        {
            get
            {
                return _editDurationCommand;
            }
        }

        public BindableMediaItem SelectedMediaItem
        {
            get { return _selectedMediaItem; }
            set
            {
                _selectedMediaItem = value;
                this._addPlayItemCommand.RaiseCanExecuteChanged();
            }
        }

        //public CollectionView PlayItemsView
        //{
        //    get
        //    {
        //        return _playItemsView;
        //    }
        //}

        public BindablePlayItem SelectedPlayItem
        {
            get { return _selectedPlayItem; }
            set
            {
                _selectedPlayItem = value;
                this._addPlayItemCommand.RaiseCanExecuteChanged();
                this._deletePlayItemCommand.RaiseCanExecuteChanged();

                _forcePlayCommand.RaiseCanExecuteChanged();

                this.OnPropertyChanged(() => this.SelectedPlayItem);
            }
        }

        internal void ChangeSource(BindablePlayItem playItem, MediaItem mediaItem)
        {
            //var action = new PlaybillChangeMediaItemAction();
            //action.PlayItem = playItem.PlayItem;
            //action.NewMediaItem = mediaItem;

            //try
            //{
            //    _playbill.DoChangeMediaItemAction(action);
            //}
            //catch(Exception ex)
            //{

            //}            
        }

        public FCSPlayout.WPF.Core.PlayScheduleInfo PlayScheduleInfo
        {
            get { return _playScheduleInfo; }
            set
            {
                if (_playScheduleInfo != value)
                {
                    _playScheduleInfo = value;
                    _addPlayItemCommand.RaiseCanExecuteChanged();
                }
            }
        }

        internal bool CanChangeSource(object item)
        {
            BindablePlayItem playItem = item as BindablePlayItem;
            return playItem != null; // && !playItem.PlaybillItem.IsNull();
        }

        public ICommand AddPlayItemCommand
        {
            get { return _addPlayItemCommand; }
        }

        public ICommand DeletePlayItemCommand
        {
            get { return _deletePlayItemCommand; }
        }

        public ICommand LoadPlaybillCommand
        {
            get
            {
                return _loadPlaybillCommand;
            }
        }

        public InteractionRequest<Notification> DisplayMessageInteractionRequest
        {
            get; set;
        }

        //private Playbill Playbill
        //{
        //    get
        //    {
        //        return _playbill;
        //    }

        //    set
        //    {
        //        if (_playbill != value)
        //        {
        //            _playbill = value;
        //        }
        //    }
        //}

        public IPlayItemCollection2 PlayItemCollection
        {
            get
            {
                return _playItemCollection;
            }
        }

        //public ObservableCollection<BindablePlayItem> BindablePlayItems
        //{
        //    get
        //    {
        //        return _bindablePlayItems;
        //    }
        //}

        public InteractionRequest<Notification> ForcePlayRequest
        {
            get
            {
                return _forcePlayRequest;
            }
        }

        public DelegateCommand ForcePlayCommand
        {
            get
            {
                return _forcePlayCommand;
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

        public ICommand ChangeToAutoModeCommand
        {
            get
            {
                return _changeToAutoModeCommand;
            }
        }

        internal InteractionRequest<EditDateTimeConfirmation> EditDateTimeInteractionRequest { get; set; }
        internal InteractionRequest<EditCGItemsConfirmation> EditCGItemsInteractionRequest { get; set; }
        public ICommand ChangeStartTimeCommand
        {
            get
            {
                return _changeStartTimeCommand;
            }
        }

        public ICommand EditCGItemsCommand
        {
            get
            {
                return _editCGItemsCommand;
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

        internal void UpdateMenuCommands()
        {
            _editMediaItemCommand.RaiseCanExecuteChanged();
            _editDurationCommand.RaiseCanExecuteChanged();

            _moveDownCommand.RaiseCanExecuteChanged();
            _moveUpCommand.RaiseCanExecuteChanged();
            _changeToAutoModeCommand.RaiseCanExecuteChanged();
            _changeStartTimeCommand.RaiseCanExecuteChanged();
            _changeToBreakModeCommand.RaiseCanExecuteChanged();
            _changeToTimingModeCommand.RaiseCanExecuteChanged();
        }

        #region Command Method
        private bool CanAddPlayItem()
        {
            if (this.PlayScheduleInfo == null || this.SelectedMediaItem == null)
            {
                return false;
            }

            if (this.PlayScheduleInfo.Mode == PlayScheduleMode.Auto)
            {
                //if (this.SelectedPlayItem == null)
                //{
                //    return false;
                //}

                //if (this.SelectedPlayItem.ScheduleMode == PlayScheduleMode.TimingBreak)
                //{
                //    return false;
                //}

                //if(this.SelectedPlayItem != null && ((IPlayItemAdapter)this.SelectedPlayItem).Adaptee.IsAutoFillItem())
                //{
                //    return false;
                //}
            }
            return true;
        }

        private void AddPlayItem()
        {
            if (CanAddPlayItem())
            {
                BindableMediaItem bindableMediaItem = this.SelectedMediaItem as BindableMediaItem;

                if (bindableMediaItem != null)
                {
                    PlaybillItem billItem = null;
                    

                    try
                    {
                        switch (this.PlayScheduleInfo.Mode)
                        {
                            case PlayScheduleMode.Timing:
                                billItem = PlaybillItem.Timing(bindableMediaItem.MediaItem, this.PlayScheduleInfo.StartTime.Value);
                                using (var editor = this.Playlist.Edit())
                                {
                                    editor.InsertTiming(billItem);
                                }
                                break;
                            case PlayScheduleMode.Auto:
                                if (this.SelectedPlayItem != null)
                                {
                                    billItem = PlaybillItem.Auto(bindableMediaItem.MediaItem);
                                    using (var editor = this.Playlist.Edit())
                                    {
                                        editor.AddAutoAfter(this.SelectedPlayItem.PlayItem, (AutoPlaybillItem)billItem);
                                    }
                                }
                                break;
                            case PlayScheduleMode.TimingBreak:
                                billItem = PlaybillItem.TimingBreak(bindableMediaItem.MediaItem, this.PlayScheduleInfo.StartTime.Value);
                                using (var editor = this.Playlist.Edit())
                                {
                                    editor.InsertTiming(billItem);
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        RaiseDisplayMessageInteractionRequest("错误", ex.Message);
                    }
                }
            }
        }

        internal void OnForcePlay(IPlayItem currentPlayItem,PlayRange remained,IPlayItem forcedItem)
        {
            //this.Playbill.OnForcePlay(currentPlayItem, remained, forcedItem);
        }

        //private void LoadPlaybill()
        //{
        //    OpenFileInteractionRequest.Raise(new OpenFileDialogConfirmation()
        //    { Title = "打开", Multiselect = false, Filter = "XML文件 (*.xml)|*.xml" }, (n) =>
        //    {
        //        if (n.Confirmed)
        //        {

        //            var entity = new PlaybillEntity();
        //            entity.Load(n.FileName);
        //            var playItemList = new List<IPlayItem>();

        //            Playbill = Playbill.FromEntity(entity, playItemList);
        //        }
        //    });
        //}

        private bool CanDeletePlayItem()
        {
            return this.SelectedPlayItem != null;
            //return false; // this.Playbill.CanRemove(this.SelectedPlayItem.PlaybillItem);
        }

        private void DeletePlayItem()
        {
            if (CanDeletePlayItem())
            {
                try
                {
                    //this.Playbill.Remove(this.SelectedPlayItem.PlayItem);
                }
                catch (Exception ex)
                {
                    RaiseDisplayMessageInteractionRequest("错误", ex.Message);
                }
            }
        }

        private bool CanForcePlay()
        {
            return this.SelectedPlayItem != null;
        }

        private void ForcePlay()
        {
            if (CanForcePlay())
            {
                _forcePlayRequest.Raise(new Notification { Content=this.SelectedPlayItem }, n => { });
            }
        }
        #endregion

        private void RaiseDisplayMessageInteractionRequest(string title, string message)
        {
            if (DisplayMessageInteractionRequest != null)
            {
                DisplayMessageInteractionRequest.Raise(new Notification { Title = title, Content = message });
            }
        }

        private bool CanEditDuration(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return false; // playItem != null && !playItem.IsAutoPatching() && (playItem.Source is IChannelMediaSource);
        }

        private void EditDuration(object cmdParameter)
        {
            if (CanEditDuration(cmdParameter))
            {
                BindablePlayItem playItem = cmdParameter as BindablePlayItem;

                if (playItem != null)
                {
                    //EditDurationInteractionRequest.Raise(new EditDurationConfirmation(playItem.MediaItem)
                    //{ Title = playItem.Title },
                    //n =>
                    //{
                    //    if (n.Confirmed)
                    //    {
                    //        var action = new PlaybillChangePlayRangeAction();
                    //        action.NewPlayRange = new PlayRange(playItem.PlayRange.MaxDuration,TimeSpan.Zero, n.Duration);
                    //        action.PlayItem = playItem.PlayItem;

                    //        try
                    //        {
                    //            this.Playbill.DoChangePlayRangeAction(action);
                    //            //_playbill.ChangePlayRange(playItem/*.PlaybillItem*/, n.PlayRange);
                    //        }
                    //        catch (Exception ex)
                    //        {

                    //        }
                    //    }
                    //});
                }
                else
                {
                }
            }
        }

        private bool CanEditMediaItem(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return false; // playItem != null && !playItem.IsAutoPatching() && (playItem.Source is IFileMediaSource);
        }

        private void EditMediaItem(object cmdParameter)
        {
            if (CanEditMediaItem(cmdParameter))
            {
                BindablePlayItem playItem = cmdParameter as BindablePlayItem;

                if (playItem != null)
                {
                    //var strPath = ((FileMediaSource)playItem.MediaItem.Source).Path;

                    //EditMediaItemInteractionRequest.Raise(new EditMediaItemConfirmation(strPath, playItem.MediaItem.PlayRange)
                    //{ Title = playItem.Title },
                    //n =>
                    //{
                    //    if (n.Confirmed)
                    //    {
                    //        var action = new PlaybillChangePlayRangeAction();
                    //        action.NewPlayRange = n.PlayRange;
                    //        action.PlayItem = playItem.PlayItem;
                            
                    //        try
                    //        {
                    //            this.Playbill.DoChangePlayRangeAction(action);
                    //        }
                    //        catch(Exception ex)
                    //        {
                                
                    //        }
                    //    }
                    //});
                }
                else
                {
                }
            }
        }

        private bool CanMoveDown(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return false; // playItem != null && !playItem.PlaybillItem.IsNull() && _playbill.CanMoveDown(playItem.PlaybillItem);
        }

        private void MoveDown(object cmdParameter)
        {
            if (CanMoveDown(cmdParameter))
            {
                //_playbill.MoveDown(((BindablePlayItem)cmdParameter).PlaybillItem);
            }
        }

        private bool CanMoveUp(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return false; // playItem != null && !playItem.PlaybillItem.IsNull() && _playbill.CanMoveUp(playItem.PlaybillItem);
        }

        private void MoveUp(object cmdParameter)
        {
            if (CanMoveUp(cmdParameter))
            {
                //_playbill.MoveUp(((BindablePlayItem)cmdParameter).PlaybillItem);
            }
        }

        private bool CanChangeToAutoMode(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return false; // playItem != null && !playItem.IsAutoPatching() && !playItem.IsNoTiming();
        }

        private void ChangeToAutoMode(object cmdParameter)
        {
            if (CanChangeToAutoMode(cmdParameter))
            {
                //var action = new PlaybillChangeScheduleModeAction();
                //action.NewScheduleMode = PlayScheduleMode.Auto;
                //action.PlayItem = ((BindablePlayItem)cmdParameter).PlayItem;
                //this.Playbill.DoChangeScheduleModeAction(action);
            }
        }

        private bool CanChangeToBreakMode(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return false; // playItem != null && !playItem.IsAutoPatching() && !playItem.IsTimingBreak();
        }

        private void ChangeToBreakMode(object cmdParameter)
        {
            if (CanChangeToBreakMode(cmdParameter))
            {
                BindablePlayItem playItem = (BindablePlayItem)cmdParameter;

                

                this.EditDateTimeInteractionRequest.Raise(new EditDateTimeConfirmation
                {
                    Time = playItem.StartTime,
                    Title = "更改为定时插播"
                },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            //var action = new PlaybillChangeScheduleModeAction();
                            //action.NewScheduleMode = PlayScheduleMode.TimingBreak;
                            //action.PlayItem = playItem.PlayItem;
                            //action.NewStartTime = c.Time;
                            //this.Playbill.DoChangeScheduleModeAction(action);
                            //ChangeToBreakMode(playItem.PlaybillItem, c.Time);
                        }
                    });
                //_playbill.ChangeToAutoMode(((BindablePlayItem)cmdParameter).PlaybillItem);
            }
        }

        private bool CanChangeToTimingMode(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return false; // playItem != null && !playItem.PlaybillItem.IsNull() && _playbill.CanChangeToTimingMode(playItem.PlaybillItem);
        }

        private void ChangeToTimingMode(object cmdParameter)
        {
            if (CanChangeToTimingMode(cmdParameter))
            {
                BindablePlayItem playItem = (BindablePlayItem)cmdParameter;

                this.EditDateTimeInteractionRequest.Raise(new EditDateTimeConfirmation { Time = playItem.StartTime, Title = "更改为定时播" },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            //var action = new PlaybillChangeScheduleModeAction();
                            //action.NewScheduleMode = PlayScheduleMode.Timing;
                            //action.PlayItem = playItem.PlayItem;
                            //action.NewStartTime = c.Time;
                            //this.Playbill.DoChangeScheduleModeAction(action);
                            //ChangeToTimingMode(playItem.PlaybillItem, c.Time);
                        }
                    });
                //_playbill.ChangeToAutoMode(((BindablePlayItem)cmdParameter).PlaybillItem);
            }
        }


        private bool CanChangeStartTime(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return false; // playItem != null && !playItem.IsNoTiming(); // !playItem.PlaybillItem.IsNull() && _playbill.CanChangeStartTime(playItem.PlaybillItem);
        }

        private void ChangeStartTime(object cmdParameter)
        {
            if (CanChangeStartTime(cmdParameter))
            {
                //ITimingPlaybillItem timingItem = ((BindablePlayItem)cmdParameter).PlaybillItem as ITimingPlaybillItem;
                BindablePlayItem timingItem = (BindablePlayItem)cmdParameter;
                this.EditDateTimeInteractionRequest.Raise(new EditDateTimeConfirmation { Time = timingItem.StartTime, Title = "编辑开始时间" },
                    c =>
                    {
                        if (c.Confirmed)
                        {

                            //var action = new PlaybillChangeStartTimeAction();
                            //action.NewStartTime = c.Time;
                            //action.PlayItem = timingItem.PlayItem;

                            //try
                            //{
                            //    this.Playbill.DoChangeStartTimeAction(action);
                            //}
                            //catch(Exception ex)
                            //{
                            //    this.DisplayMessageInteractionRequest.Raise(new Notification { Content = ex.Message, Title = "错误" });
                            //}
                            //ChangeStartTime(((BindablePlayItem)cmdParameter).PlaybillItem, c.Time);
                        }
                    });
            }
        }

        private bool CanEditCGItems(object cmdParameter)
        {
            BindablePlayItem playItem = cmdParameter as BindablePlayItem;
            return playItem != null;
        }

        private void EditCGItems(object cmdParameter)
        {
            if (CanEditCGItems(cmdParameter))
            {
                var timingItem = (BindablePlayItem)cmdParameter;

                this.EditCGItemsInteractionRequest.Raise(new EditCGItemsConfirmation { Title = "编辑字幕", Items = timingItem.CGItems },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            timingItem.CGItems = c.Items;
                        }
                    });
            }
        }


        #region
        public ICommand SavePlaybillCommand
        {
            get
            {
                return _savePlaybillCommand;
            }
        }

        public InteractionRequest<LoadPlaybillConfirmation> LoadPlaybillInteractionRequest
        { get; internal set; }
        public IPlaylist2 Playlist { get; internal set; }

        private bool CanSavePlaybill()
        {
            return true; // this.Playbill.CanSave();
            //return _playItemCollection.Count > 0;
        }

        //private FCSPlayout.Domain.Entities.PlayoutRepository _repository = new FCSPlayout.Domain.Entities.PlayoutRepository();
        private void SavePlaybill()
        {
            //_repository.Save(this.Playbill);
        }

        private bool CanLoadPlaybill()
        {
            return true;
        }

        private void LoadPlaybill()
        {
            if (CanLoadPlaybill())
            {
                this.LoadPlaybillInteractionRequest.Raise(new LoadPlaybillConfirmation { Title="选择节目单",Playbills=LoadPlaybills() },
                    c=> 
                    {
                        if (c.Confirmed)
                        {
                            var bill = c.SelectedPlaybill;

                            //_playItemList.Clear();
                            //var bill2=FCSPlayout.Playbill.Load(bill.Id);
                            //this.Playbill.Append(bill2);
                        }
                    });
                //this.Playbill.Load();
            }
        }

        private IEnumerable<BindablePlaybill> LoadPlaybills()
        {
            List<BindablePlaybill> result = new List<BindablePlaybill>();
            //using(var ctx=new FCSPlayout.PlayoutDbContext())
            //{
            //    var temp = ctx.Playbills.ToList();
            //    foreach(var item in temp)
            //    {
            //        result.Add(new BindablePlaybill(item));
            //    }
            //}
            return result;
        }
        #endregion
    }
}
