//using FCSPlayout.Domain;

using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.MediaFileImporter;
using FCSPlayout.WPF.Core;
using FCSPlayout.WPFApp.Views;
//using FCSPlayout.WPFApp.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FCSPlayout.WPFApp.ViewModels
{
    public class MediaItemListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<BindableMediaFileItem> _mediaItemCollection;
        private BindableMediaFileItem _selectedMediaItem;
        
        private readonly DelegateCommand _addMediaItemCommand;
        private readonly DelegateCommand _deleteMediaItemCommand;
        private readonly DelegateCommand<object> _editMediaItemCommand;
        private readonly DelegateCommand _saveMediaItemsCommand;

        private IUploadProgressFeedback _progressFeedback;
        private DelegateBackgroundWorker _worker;
        private BindableMediaFileItem _currentItem;

        public MediaItemListViewModel()
        {
            _worker = new DelegateBackgroundWorker();
            _worker.ProgressChangedHandler = this.ReportUploadProgress;
            _worker.RunCompletedHandler = this.OnUploadCompleted;
         

            _mediaItemCollection = new ObservableCollection<BindableMediaFileItem>();
            _addMediaItemCommand = new DelegateCommand(AddMediaItem);
            _deleteMediaItemCommand = new DelegateCommand(DeleteMediaItem, CanDeleteMediaItem);
            _editMediaItemCommand = new DelegateCommand<object>(EditMediaItem, CanEditMediaItem);
            _saveMediaItemsCommand = new DelegateCommand(SaveMediaItems, CanSaveMediaItems);
        }

        private bool CanDeleteMediaItem()
        {
            return this.SelectedMediaItem != null && this.SelectedMediaItem!=_currentItem;
        }

        private void DeleteMediaItem()
        {
            if (CanDeleteMediaItem())
            {
                _mediaItemCollection.Remove(this.SelectedMediaItem);
                this.SelectedMediaItem = null;
                _saveMediaItemsCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanSaveMediaItems()
        {
            return _mediaItemCollection.Count > 0;
        }

        private void SaveMediaItems()
        {
            if (CanSaveMediaItems())
            {
                ProgressFeedback.Open();
                if (_mediaItemCollection.Count > 0)
                {
                    var item = _mediaItemCollection[0];
                    Upload(item);      
                }
            }
        }
     
        public ObservableCollection<BindableMediaFileItem> MediaItemView
        {
            get
            {
                return _mediaItemCollection;
            }
        }

        public BindableMediaFileItem SelectedMediaItem
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

        

        public IUploadProgressFeedback ProgressFeedback
        {
            get
            {
                return _progressFeedback ?? NullUploadProgressFeedback.Instance;
            }

            set
            {
                _progressFeedback = value;
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
                        var fileNames = n.FileNames.ToList();
                        for (int j=0;j<fileNames.Count;j++)
                        {
                            var fileName = fileNames[j];

                            //TimeSpan duration = GetFileDuration(fileName);
                            _mediaItemCollection.Add(new BindableMediaFileItem(fileName));
                            //PlaybillEditor.datagridHepler.SetShowRowIndexProperty(MediaItemListView.mv.dgMediaItem, true);
                            _saveMediaItemsCommand.RaiseCanExecuteChanged();

                        }
                    }
                });
            }
        }

        private bool CanEditMediaItem(object cmdParameter)
        {
            Views.DataGridRowDoubleClickEventArgs args = cmdParameter as Views.DataGridRowDoubleClickEventArgs;
            if (args != null)
            {
                BindableMediaFileItem item = args.Item as BindableMediaFileItem;
                return item != null;
            }
            return false;
        }

        BindableMediaFileItem bindable;
        

        private void EditMediaItem(object cmdParameter)
        {
            if (CanEditMediaItem(cmdParameter))
            {
                Views.DataGridRowDoubleClickEventArgs args = cmdParameter as Views.DataGridRowDoubleClickEventArgs;
                bindable = args.Item as BindableMediaFileItem;
                //System.Diagnostics.Debug.WriteLine(cmdParameter);
                //bindable = this.SelectedMediaItem as BindableMediaFileItem;
                if (bindable != null)
                {
                    var strPath = bindable.FilePath; // ((FileMediaSource)).Path;
                    var range = new PlayRange(bindable.StartPosition, bindable.StopPosition - bindable.StartPosition);// new PlayRange(bindable.Duration, bindable.StartPosition, bindable.StopPosition);



                    //MediaItemListView.mv.mw1.Init(strPath, range, PlayoutRepository.GetMPlaylistSettings(),
                    //    ()=>bindable!=null, 
                    //    (inPos)=> {
                    //        var oldStop = bindable.PlayRange.StopPosition;
                    //        var newInPos = TimeSpan.FromSeconds(inPos);
                    //        var duration = oldStop - newInPos;
                    //        bindable.PlayRange = new PlayRange(newInPos,duration<TimeSpan.Zero ? TimeSpan.Zero : duration);
                    //            },
                    //    () => bindable != null, 
                    //    (outPos) => {
                    //        var oldStart = bindable.PlayRange.StartPosition;
                    //        var newOutPos = TimeSpan.FromSeconds(outPos);
                    //        var duration = newOutPos - oldStart;
                    //        bindable.PlayRange = new PlayRange(oldStart, duration < TimeSpan.Zero ? TimeSpan.Zero : duration);
                    //    });
                    //MediaItemListView.mv.mw1.play();            
                }
                else
                {
                }
            }
        }

        #region
        private void Upload(BindableMediaFileItem item)
        {
            _currentItem = item;
            _worker.State = item;
            _worker.Run((c) => { MediaFileService.UploadFile(item.FilePath, item.FileName, c); });
        }

        private void ReportUploadProgress(int progress,object state)
        {
            ProgressFeedback.Report(progress, (MediaFileStorage)state);
        }
    

        private void OnUploadCompleted(Exception error,bool cancelled,object result)
        {
            BindableMediaFileItem item = (BindableMediaFileItem)_worker.State;

            if (error==null && !cancelled)
            {
                var entity = item.Entity;
                MediaFileService.Add(item.Entity, App.Name);

                _worker.State = null;
                _mediaItemCollection.Remove(item);
            }
            else
            {
                _worker.State = null;
                _mediaItemCollection.Remove(item);

                // 放到尾部。
                _mediaItemCollection.Add(item);
            }

            //PlaybillEditor.datagridHepler.SetShowRowIndexProperty(MainWindow.mw.dgMediaItem1, true);

            //MainWindow.mw.dgMediaItem1.ItemsSource =list;

            if (_mediaItemCollection.Count > 0)
            {
                item = _mediaItemCollection[0];
                Upload(item);
            }
            else
            {
                _currentItem = null;
                ProgressFeedback.Close();
            }
        }        
        #endregion
    }
}
