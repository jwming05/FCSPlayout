using FCSPlayout.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace FCSPlayout.WPFApp.ViewModels
{
    class ChannelSettingsViewModel:BindableBase
    {
        private ICommand _addCommand;
        private DelegateCommand _removeCommand;
        private ChannelInfo _selectedChannelInfo;
        private List<ChannelInfo> _removedItems = new List<ChannelInfo>();
        private List<ChannelInfo> _updatedItems = new List<ChannelInfo>();

        public ChannelSettingsViewModel()
        {
            this.ChannelInfos = new ObservableCollection<ChannelInfo>(PlayoutRepository.GetChannelInfos());
            _removeCommand = new DelegateCommand(ExecuteRemove,CanExecuteRemove);
            _addCommand = new DelegateCommand(ExecuteAdd);
        }

        public ObservableCollection<ChannelInfo> ChannelInfos
        {
            get; private set;
        }

        public ChannelInfo SelectedChannelInfo
        {
            get { return _selectedChannelInfo; }
            set
            {
                if (_selectedChannelInfo != value)
                {
                    _selectedChannelInfo = value;
                    this.OnPropertyChanged("SelectedChannelInfo");
                    this.OnPropertyChanged("CanEditCurrent");

                    this.OnPropertyChanged("CurrentName");
                    //this.OnPropertyChanged("CurrentAddress");
                    _removeCommand.RaiseCanExecuteChanged();
                }
            }
        }

        internal void Save()
        {
            PlayoutRepository.Save(this.ChannelInfos.Except(_updatedItems).Where(i=>i.IsNew() && !string.IsNullOrEmpty(i.Title)), 
                _updatedItems,_removedItems);
            _updatedItems.Clear();
            _removedItems.Clear();
        }

        public string CurrentName
        {
            get
            {
                return this.SelectedChannelInfo != null ? this.SelectedChannelInfo.Title : string.Empty;
            }
            set
            {
                if (this.SelectedChannelInfo != null && this.SelectedChannelInfo.Title!=value)
                {
                    this.SelectedChannelInfo.Title = value;

                    TryRecordCurrentItemAsDirty();
                }
            }
        }

        private void TryRecordCurrentItemAsDirty()
        {
            if (!this.SelectedChannelInfo.IsNew() && !_updatedItems.Contains(this.SelectedChannelInfo))
            {
                _updatedItems.Add(this.SelectedChannelInfo);
            }
        }

        //public string CurrentAddress
        //{
        //    get
        //    {
        //        return this.SelectedChannelInfo != null ? this.SelectedChannelInfo.Address : string.Empty;
        //    }
        //    set
        //    {
        //        if (this.SelectedChannelInfo != null && this.SelectedChannelInfo.Address != value)
        //        {
        //            this.SelectedChannelInfo.Address = value;

        //            TryRecordCurrentItemAsDirty();
        //        }
        //    }
        //}

        public ICommand AddCommand
        { get { return _addCommand; } }

        public ICommand RemoveCommand
        { get { return _removeCommand; } }

        public bool CanEditCurrent
        { get { return this.SelectedChannelInfo != null && !this.SelectedChannelInfo.Special; } }

        private void ExecuteRemove()
        {
            if (CanExecuteRemove())
            {
                var item = this.SelectedChannelInfo;
                if (!item.IsNew())
                {
                    _removedItems.Add(item);
                }

                this.ChannelInfos.Remove(item);
                this.SelectedChannelInfo = null;
            }
        }

        private bool CanExecuteRemove()
        {
            return this.SelectedChannelInfo != null && !this.SelectedChannelInfo.Special;
        }

        private void ExecuteAdd()
        {
            var item = new ChannelInfo();
            this.ChannelInfos.Add(item);
            this.SelectedChannelInfo = item;
        }
    }
}
