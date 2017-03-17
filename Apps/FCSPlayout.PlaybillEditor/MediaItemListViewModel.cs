using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using FCSPlayout.WPFApp.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace FCSPlayout.PlaybillEditor
{
    public class MediaItemListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<BindableMediaItem> _mediaItemCollection;
        private readonly CollectionView _mediaItemView;
        private BindableMediaItem _selectedMediaItem;
        private readonly DelegateCommand _addMediaItemCommand;
        private readonly DelegateCommand _deleteMediaItemCommand;
        private readonly DelegateCommand<object> _editMediaItemCommand;
        private readonly DelegateCommand _saveMediaItemsCommand;
        private readonly DelegateCommand _loadMediaItemsCommand;

        
        private InteractionRequest<AddNullMediaItemConfirmation> _addNullMediaItemInteractionRequest;
        private InteractionRequest<AddChannelMediaItemsConfirmation> _addChannelMediaItemsInteractionRequest;
        private readonly DelegateCommand _addNullMediaItemCommand;
        private readonly DelegateCommand _addChannelMediaItemsCommand;
        //private XmlMediaItemRepository _mediaItemRepository = new XmlMediaItemRepository();
        public MediaItemListViewModel()
        {
            _mediaItemCollection = new ObservableCollection<BindableMediaItem>();
            _mediaItemView = new CollectionView(_mediaItemCollection);

            
            _addNullMediaItemInteractionRequest = new InteractionRequest<AddNullMediaItemConfirmation>();
            _addChannelMediaItemsInteractionRequest = new InteractionRequest<AddChannelMediaItemsConfirmation>();

            _addMediaItemCommand = new DelegateCommand(AddMediaItem);
            _addNullMediaItemCommand= new DelegateCommand(AddNullMediaItem);
            _addChannelMediaItemsCommand = new DelegateCommand(AddChannelMediaItems);

            _deleteMediaItemCommand = new DelegateCommand(DeleteMediaItem, CanDeleteMediaItem);
            _editMediaItemCommand = new DelegateCommand<object>(EditMediaItem, CanEditMediaItem);

            _saveMediaItemsCommand = new DelegateCommand(SaveMediaItems, CanSaveMediaItems);
            _loadMediaItemsCommand = new DelegateCommand(LoadMediaItems);
        }

        public ICommand AddChannelMediaItemsCommand
        {
            get { return _addChannelMediaItemsCommand; }
        }

        private bool CanDeleteMediaItem()
        {
            return this.SelectedMediaItem != null;
        }

        private void DeleteMediaItem()
        {
            if (CanDeleteMediaItem())
            {
                _mediaItemCollection.Remove(this.SelectedMediaItem);
                _saveMediaItemsCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanSaveMediaItems()
        {
            return _mediaItemCollection.Count > 0;
        }

        //private Domain.Entities.PlayoutRepository _repository= new Domain.Entities.PlayoutRepository();
        private void SaveMediaItems()
        {
            if (CanSaveMediaItems())
            {
                //_repository.AddMediaItems(_mediaItemCollection.Select(i=>(Domain.Entities.IMediaItemEntity)i.MediaItem));

                //this.SaveFileInteractionRequest.Raise(new SaveFileDialogConfirmation() { Title = "保存", Filter = "XML文件 (*.xml)|*.xml" }, n =>
                //{
                //    if (n.Confirmed)
                //    {
                //        //_mediaItemRepository.Save(n.FileName, _mediaItemCollection.Select(i => ((BindableMediaItem)i).MediaItem));
                //    }
                //});
            }
        }

        private IEnumerable<FCSPlayout.Entities.MediaFileEntity> LoadEntities()
        {
            using(var ctx=new PlayoutDbContext())
            {
                return ctx.MediaFiles.Where(i=>!i.Deleted).OrderByDescending(m => m.CreationTime).ToArray();
            }
        }

        private IEnumerable<FCSPlayout.Entities.MediaFileEntity> LoadEntities(string titleSearch, int rowIndex, int rowCount, out int totalRowCount)
        {
            using (var ctx = new PlayoutDbContext())
            {
                if (string.IsNullOrEmpty(titleSearch))
                {
                    totalRowCount = ctx.MediaFiles.Count();
                    return ctx.MediaFiles.OrderByDescending(m => m.CreationTime)
                        .OrderBy(m=>m.Id).Skip(rowIndex).Take(rowCount).ToArray();
                }
                else
                {
                    totalRowCount = ctx.MediaFiles.Where(m => m.Title.Contains(titleSearch)).Count();
                    return ctx.MediaFiles.Where(m => m.Title.Contains(titleSearch))
                        .OrderByDescending(m => m.CreationTime)
                        .OrderBy(m => m.Id).Skip(rowIndex).Take(rowCount).ToArray();
                }
            }
        }

        private void LoadMediaItems()
        {
            foreach(var item in LoadEntities())
            {
                _mediaItemCollection.Add(new BindableMediaItem(MediaItemUtils.FromEntity(item)));

                datagridHepler.SetShowRowIndexProperty(MediaItemListView.mv.dgMediaItem, true);
            }

            //this.OpenFileInteractionRequest.Raise(new OpenFileDialogConfirmation() { Title = "打开", Multiselect = true, Filter = "XML文件 (*.xml)|*.xml" }, n =>
            //{
            //    if (n.Confirmed)
            //    {
            //        foreach (var fileName in n.FileNames)
            //        {
            //            //try
            //            //{
            //            //    var mediaItems = _mediaItemRepository.Load(fileName);
            //            //    foreach (var item in mediaItems)
            //            //    {
            //            //        _mediaItemCollection.Add(new BindableMediaItem(item));
            //            //    }
            //            //}
            //            //catch
            //            //{

            //            //}
            //        }
            //    }
            //});
        }

        public CollectionView MediaItemView
        {
            get
            {
                return _mediaItemView;
            }
        }

        public BindableMediaItem SelectedMediaItem
        {
            get { return _selectedMediaItem; }
            set
            {
                _selectedMediaItem = value;
                _editMediaItemCommand.RaiseCanExecuteChanged();
                _deleteMediaItemCommand.RaiseCanExecuteChanged();
                OnSelectedMediaItemChanged();
            }
        }

        public event EventHandler SelectedMediaItemChanged;
        private void OnSelectedMediaItemChanged()
        {
            if (SelectedMediaItemChanged != null)
            {
                SelectedMediaItemChanged(this, EventArgs.Empty);
            }
        }

        public ICommand AddMediaItemCommand
        {
            get
            {
                return _addMediaItemCommand;
            }
        }

        public ICommand EditMediaItemCommand
        {
            get
            {
                return _editMediaItemCommand;
            }
        }

        public ICommand DeleteMediaItemCommand
        {
            get
            {
                return _deleteMediaItemCommand;
            }
        }

        public ICommand SaveMediaItemsCommand
        {
            get
            {
                return _saveMediaItemsCommand;
            }
        }

        public ICommand LoadMediaItemsCommand
        {
            get
            {
                return _loadMediaItemsCommand;
            }
        }



        public InteractionRequest<AddNullMediaItemConfirmation> AddNullMediaItemInteractionRequest
        {
            get { return _addNullMediaItemInteractionRequest; }
        }

        public InteractionRequest<AddChannelMediaItemsConfirmation> AddChannelMediaItemsInteractionRequest
        {
            get { return _addChannelMediaItemsInteractionRequest; }
        }

        public DelegateCommand AddNullMediaItemCommand
        {
            get
            {
                return _addNullMediaItemCommand;
            }
        }

        private void AddMediaItem()
        {
            if (OpenFileInteractionRequest != null)
            {
                OpenFileInteractionRequest.Raise(new OpenFileDialogConfirmation() { Filter = "媒体文件(*.*)|*.*", Multiselect = true },
                n =>
                {
                    if (n.Confirmed)
                    {
                        //var fileNames = n.FileNames.ToList();
                        //for (int j=0;j<fileNames.Count;j++)
                        //{
                        //    var fileName = fileNames[j];

                        //    var mediaItem = new MediaItem(new FCSPlayout.Domain.FileMediaSource(fileName,GetDuration(fileName))); 
                        //    _mediaItemCollection.Add(new BindableMediaItem(mediaItem));

                        //    _saveMediaItemsCommand.RaiseCanExecuteChanged();
                        //}
                    }
                });
            }
        }

        private TimeSpan GetDuration(string fileName)
        {
            return MediaFileDurationGetter.Current.GetDuration(fileName);
        }

        private void AddNullMediaItem()
        {
            if (AddNullMediaItemInteractionRequest != null)
            {
                AddNullMediaItemInteractionRequest.Raise(new AddNullMediaItemConfirmation
                {
                    Title = "添加空项"
                }, c =>
                {
                    if (c.Confirmed)
                    {
                        //var mediaItem = MediaItem.CreateNull(c.Caption, c.Duration, false);

                        //_mediaItemCollection.Add(new BindableMediaItem(mediaItem));
                    }
                });
            }
        }

        private void AddChannelMediaItems()
        {
            if (AddChannelMediaItemsInteractionRequest != null)
            {
                AddChannelMediaItemsInteractionRequest.Raise(new AddChannelMediaItemsConfirmation
                {
                    Title = "添加"
                }, c =>
                {
                    if (c.Confirmed && c.SelectedChannel != null)
                    {
                        var source = new ChannelMediaSource(c.SelectedChannel);
                        var mediaItem = new MediaItem(source, new PlayRange(/*source.Duration,*/TimeSpan.Zero, TimeSpan.FromHours(1)));

                        _mediaItemCollection.Add(new BindableMediaItem(mediaItem));
                    }
                });
            }
        }

        private bool CanEditMediaItem(object cmdParameter)
        {
            DataGridRowDoubleClickEventArgs args = cmdParameter as DataGridRowDoubleClickEventArgs;
            if (args != null)
            {
                BindableMediaItem item = args.Item as BindableMediaItem;
                return item != null;
            }
            return false;
        }

        private void EditMediaItem(object cmdParameter)
        {
            if (CanEditMediaItem(cmdParameter))
            {
                DataGridRowDoubleClickEventArgs args = cmdParameter as DataGridRowDoubleClickEventArgs;
                BindableMediaItem bindable = args.Item as BindableMediaItem;
                if (bindable != null)
                {
                    var strPath = ((FileMediaSource)bindable.MediaItem.Source).FileName;
                    EditMediaItemInteractionRequest.Raise(new EditMediaItemConfirmation(strPath, bindable.MediaItem.PlayRange/*.MediaItem*/)
                    { Title = bindable.Title },
                    n =>
                    {
                        if (n.Confirmed)
                        {
                            bindable.PlayRange = n.PlayRange;
                        }
                    });

                }
                else
                {
                }
            }
        }
    }
}
