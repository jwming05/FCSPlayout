﻿using FCSPlayout.AppInfrastructure;
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
    public partial class PlaybillViewModel : ViewModelBase,IPlayItemEditorFactory
    {
        private Playlist _playlist;
        private PlayItemCollection _playItemCollection;
        


        private BindablePlayItem _selectedPlayItem;


        
        //private readonly DelegateCommand<object> _editMediaItemCommand;
        

        

        
        
        
        
        

        
        

        private readonly DelegateCommand _savePlaybillCommand;
        private readonly DelegateCommand _loadPlaybillCommand;
        private readonly DelegateCommand _createPlaybillCommand;

        private Playbill _playbill;
        private IMediaSourceConverter _mediaSourceConverter;




        //private ObservableCollection<BindablePlayItem> _bindablePlayItemCollection;
        //private PlayItemList _playItems;
        public PlaybillViewModel(IEventAggregator eventAggregator, 
            IMediaFileImageResolver imageResolver,IMediaFilePathResolver filePathResolver, InteractionRequests interactionRequests,
            IMediaSourceConverter mediaSourceConverter)
        {
            _interactionRequests = interactionRequests;
            _playItemCollection = new PlayItemCollection(filePathResolver,this);
            
            this.ImageResolver = imageResolver;
            _mediaSourceConverter = mediaSourceConverter;

            //_playItemsView = new CollectionView(temp);

            _savePlaybillCommand = new DelegateCommand(SavePlaybill, CanSavePlaybill);
            _loadPlaybillCommand = new DelegateCommand(LoadPlaybill, CanLoadPlaybill);
            //_createPlaybillCommand = new DelegateCommand(CreatePlaybill, CanCreatePlaybill);


            //Playbill = new Playbill();
            this.Playlist = new Playlist(_playItemCollection);
            this.Playlist.EditCompleted += Playlist_EditCompleted;

            _listAdapter = new WrappedItemListAdapter<BindablePlayItem, IPlayItem>(_playItemCollection, (i) => _playItemCollection.CreateBindablePlayItem(i));
            //this.Playbill = new Playbill(_playItemCollection);
            //var collectionAdapter = new PlayItemCollectionAdapter2(_playItemCollection, (p) => new BindablePlayItem(p));
            //this.Playbill = new Playbill(collectionAdapter);

            //_bindablePlayItemCollection = new ObservableCollection<BindablePlayItem>();
            //_playItems = new PlayItemList(_bindablePlayItemCollection);
            //this.Playbill = new Playbill(_playItems);

            //this.Playbill.AfterExecuteAction += Playbill_AfterExecuteAction;
            
            //this.Playbill.PlayItemsChanged += Playbill_PlayItemsChanged;


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
            
            _editCGItemsCommand= new DelegateCommand(EditCGItems, CanEditCGItems);

            _previewCommand = new DelegateCommand<IPlayableItem>(Preview);

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PubSubEvent<AddPlayItemPayload>>().Subscribe(AddPlayItem);
        }

        





        //private void Playbill_AfterExecuteAction(object sender, AfterExecuteActionEventArgs e)
        //{
        //    this._playItemCollection.Clear();

        //    for(int i = 0; i < _playItems.Count; i++)
        //    {
        //        _playItemCollection.Add(new BindablePlayItem(_playItems[i]));
        //    }

        //    _playItems.RaiseCollectionChanged();
        //    //throw new NotImplementedException();
        //}



        //private void Playbill_PlayItemsChanged(object sender, EventArgs e)
        //{
        //    _playItemCollection.Clear();
        //    foreach(var item in this.Playbill.GetPlayItems())
        //    {
        //        _playItemCollection.Add(item);
        //    }
        //    throw new NotImplementedException();
        //}

        //public ICommand EditMediaItemCommand
        //{
        //    get
        //    {
        //        return _editMediaItemCommand;
        //    }
        //}



        //public BindableMediaItem SelectedMediaItem
        //{
        //    get { return _selectedMediaItem; }
        //    set
        //    {
        //        _selectedMediaItem = value;
        //        this._addPlayItemCommand.RaiseCanExecuteChanged();
        //    }
        //}

        //public CollectionView PlayItemsView
        //{
        //    get
        //    {
        //        return _playItemsView;
        //    }
        //}



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

        

        internal bool CanChangeSource(object item)
        {
            BindablePlayItem playItem = item as BindablePlayItem;
            return playItem != null; // && !playItem.PlaybillItem.IsNull();
        }

        //public ICommand AddPlayItemCommand
        //{
        //    get { return _addPlayItemCommand; }
        //}

        

        public ICommand LoadPlaybillCommand
        {
            get
            {
                return _loadPlaybillCommand;
            }
        }

        //public InteractionRequest<LoadPlaybillConfirmation> LoadPlaybillInteractionRequest
        //{ get; internal set; }

        

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

        //public PlayEngine.IPlaylist PlayItemList
        //{
        //    get
        //    {
        //        return _playItems;
        //    }
        //}

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
            //_editMediaItemCommand.RaiseCanExecuteChanged();
            _editDurationCommand.RaiseCanExecuteChanged();

            //_moveDownCommand.RaiseCanExecuteChanged();
            //_moveUpCommand.RaiseCanExecuteChanged();
            //_changeToAutoModeCommand.RaiseCanExecuteChanged();
            _changeStartTimeCommand.RaiseCanExecuteChanged();
            //_changeToBreakModeCommand.RaiseCanExecuteChanged();
            //_changeToTimingModeCommand.RaiseCanExecuteChanged();
        }

        //private void Playbill_PlayItemsChanged(object sender, EventArgs e)
        //{
        //    _playItemCollection.Clear();
        //    foreach (var playItem in Playbill)
        //    {
        //        //_playItemCollection.Add(new BindablePlayItem(playItem));
        //        _playItemCollection.Add(playItem);
        //    }
        //    _savePlaybillCommand.RaiseCanExecuteChanged();
        //}

        #region Command Method


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




        #endregion















        //private void ChangeToTimingMode(ISchedulablePlaybillItem playbillItem, DateTime time)
        //{
        //    try
        //    {
        //        //_playbill.ChangeToTimingMode(playbillItem, time);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.DisplayMessageInteractionRequest.Raise(new Notification { Content = ex.Message, Title = "错误" });
        //    }
        //}

        //private void ChangeToBreakMode(ISchedulablePlaybillItem playbillItem, DateTime time)
        //{
        //    try
        //    {
        //        //_playbill.ChangeToBreakMode(playbillItem, time);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.DisplayMessageInteractionRequest.Raise(new Notification { Content = ex.Message, Title = "错误" });
        //    }
        //}





        //private void ChangeStartTime(ISchedulablePlaybillItem playbillItem, DateTime time)
        //{
        //    try
        //    {
        //        //_playbill.ChangeStartTime(playbillItem, time);
        //    }
        //    catch(Exception ex)
        //    {
        //        this.DisplayMessageInteractionRequest.Raise(new Notification { Content=ex.Message,Title="错误" });
        //    }
        //}

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
        #region
        public ICommand SavePlaybillCommand
        {
            get
            {
                return _savePlaybillCommand;
            }
        }

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

        //private void CreatePlaybill()
        //{
        //    if (CanCreatePlaybill())
        //    {
        //        _playItems.Clear();
        //        this.Playbill = Playbill.Create(_playItems,DateTime.Now); // new Playbill(_playItems);
        //    }
        //}

        private bool CanSavePlaybill()
        {
            return true; // this.Playbill.CanSave();
            //return _playItemCollection.Count > 0;
        }

        private void SavePlaybill()
        {
            if (_playItemCollection.Count == 0)
            {
                MessageBox.Show("不能保存空表。");
                return;
            }

            if (_playbill == null)
            {
                _playbill = new Playbill();
            }

            _playbill.PlayItemCollection = _playItemCollection;
            _playbill.Save();
        }

        //private PlaybillEntity GetFirstOrDefault()
        //{
        //    using (var context = new PlayoutDbContext())
        //    {
        //        var billEntity= context.Playbills
        //            .Include(i => i.PlaybillItems.Select(p=>p.PlayItems))
        //            .Include(i=>i.PlaybillItems.Select(p=>p.MediaSource))
        //            .FirstOrDefault();

        //        if (billEntity != null)
        //        {
        //            billEntity.PlayItems = billEntity.PlaybillItems.SelectMany(i => i.PlayItems).ToList();
        //        }

        //        return billEntity;
        //    }
        //}



        private bool CanLoadPlaybill()
        {
            return true;
        }

        private void LoadPlaybill()
        {
            if (CanLoadPlaybill())
            {
                this.LoadPlaybillInteractionRequest.Raise(new LoadPlaybillConfirmation { Title = "选择节目单", Playbills = LoadPlaybills() },
                    c =>
                    {
                        if (c.Confirmed)
                        {
                            BindablePlaybill bill = c.SelectedPlaybill;

                            if (_playbill != null)
                            {

                            }

                            List<IPlayItem> playItems = new List<IPlayItem>();
                            _playbill=Playbill.Load(bill.Id, playItems);

                            //using(var editor = this.Playlist.Edit())
                            //{
                            //    editor.ClearAll();
                            //    editor.Append(playItems);
                            //}

                            //_playItems.Clear();
                            //this.Playbill = FCSPlayout.Playbill.Load(bill.Id, _playItems); // new Playbill(_playItems, billEntity);


                            //FCSPlayout.Playbill.Load()
                        }
                    });

                //this.Playbill.Load();

            }
        }

        public InteractionRequest<LoadPlaybillConfirmation> LoadPlaybillInteractionRequest
        { get; internal set; }

        private IEnumerable<BindablePlaybill> LoadPlaybills()
        {
            List<BindablePlaybill> result = new List<BindablePlaybill>();
            using (var ctx = new PlayoutDbContext())
            {
                var temp = ctx.Playbills.ToList();
                foreach (var item in temp)
                {
                    result.Add(new BindablePlaybill(item));
                }
            }
            return result;
        }

        
        #endregion
    }
}