using FCSPlayout.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace FCSPlayout.WPFApp.ViewModels
{
    class BMDSwitcherSettingsViewModel:BindableBase
    {
        private DelegateCommand _addCommand;
        private DelegateCommand _removeCommand;
        private DelegateCommand _useItCommand;
        private BMDSwitcherInfo _selectedBMDSwitcherInfo;
        private List<BMDSwitcherInfo> _removedItems = new List<BMDSwitcherInfo>();
        private List<BMDSwitcherInfo> _updatedItems = new List<BMDSwitcherInfo>();
        
        public BMDSwitcherSettingsViewModel()
        {
            this.SwitcherInputInfos = new ObservableCollection<BMDSwitcherInputInfo>();
            this.SwitcherInfos = new ObservableCollection<BMDSwitcherInfo>(PlayoutRepository.GetBMDSwitcherInfos());

            _removeCommand = new DelegateCommand(ExecuteRemove, CanExecuteRemove);
            _addCommand = new DelegateCommand(ExecuteAdd);
            _useItCommand = new DelegateCommand(ExecuteUseCurrent, CanExecuteUseCurrent);
            UpdateCurrentUse();
        }

        private void UpdateCurrentUse()
        {
            var currentUse = this.SwitcherInfos.SingleOrDefault(i => i.Id == SettingsManager.BMDSwitcherId);
            if (currentUse != null && !currentUse.IsNew())
            {
                this.CurrentUseSwitcher = currentUse.Name;
                OnPropertyChanged("CurrentUseSwitcher");
            }
        }

        private bool CanExecuteUseCurrent()
        {
            return this.SelectedBMDSwitcherInfo != null && !this.SelectedBMDSwitcherInfo.IsNew();
        }

        private void ExecuteUseCurrent()
        {
            if (CanExecuteUseCurrent())
            {
                SettingsManager.BMDSwitcherId = this.SelectedBMDSwitcherInfo.Id;
                UpdateCurrentUse();
            }
        }

        public string CurrentUseSwitcher
        {
            get;private set;
        }

        public ObservableCollection<BMDSwitcherInfo> SwitcherInfos
        {
            get; private set;
        }

        public ObservableCollection<BMDSwitcherInputInfo> SwitcherInputInfos
        {
            get; private set;
        }

        public BMDSwitcherInfo SelectedBMDSwitcherInfo
        {
            get { return _selectedBMDSwitcherInfo; }
            set
            {
                if (_selectedBMDSwitcherInfo != value)
                {
                    _selectedBMDSwitcherInfo = value;

                    this.SwitcherInputInfos.Clear();
                    if (_selectedBMDSwitcherInfo != null && !_selectedBMDSwitcherInfo.IsNew())
                    {
                        foreach(var inputItem in PlayoutRepository.GetSwitcherInputInfos(_selectedBMDSwitcherInfo.Id))
                        {
                            this.SwitcherInputInfos.Add(inputItem);
                        }
                    }

                    this.OnPropertyChanged("SelectedBMDSwitcherInfo");
                    this.OnPropertyChanged("CanEditCurrent");

                    this.OnPropertyChanged("CurrentName");
                    this.OnPropertyChanged("CurrentAddress");
                    _removeCommand.RaiseCanExecuteChanged();
                    _useItCommand.RaiseCanExecuteChanged();
                }
            }
        }

        internal void Save()
        {
            PlayoutRepository.Save(this.SwitcherInfos.Except(_updatedItems).Where(i=>i.IsNew() && !string.IsNullOrEmpty(i.Address)), _updatedItems,_removedItems, CreateInputInfo);
            _updatedItems.Clear();
            _removedItems.Clear();
        }

        IEnumerable<BMDSwitcherInputInfo> CreateInputInfo(BMDSwitcherInfo switcherInfo)
        {
            var switcherOperator = FCSPlayout.SwitcherManagement.SwitcherWrapper.Get(switcherInfo.Address);
            
            foreach(var inputItem in switcherOperator.GetPrograms())
            {
                yield return new BMDSwitcherInputInfo { Name=inputItem.Item2,Value=inputItem.Item1 };
            }
        }

        public string CurrentName
        {
            get
            {
                return this.SelectedBMDSwitcherInfo != null ? this.SelectedBMDSwitcherInfo.Name : string.Empty;
            }
            set
            {
                if (this.SelectedBMDSwitcherInfo != null && this.SelectedBMDSwitcherInfo.Name!=value)
                {
                    this.SelectedBMDSwitcherInfo.Name = value;

                    TryRecordCurrentItemAsDirty();
                }
            }
        }

        private void TryRecordCurrentItemAsDirty()
        {
            if (!this.SelectedBMDSwitcherInfo.IsNew() && !_updatedItems.Contains(this.SelectedBMDSwitcherInfo))
            {
                _updatedItems.Add(this.SelectedBMDSwitcherInfo);
            }
        }

        public string CurrentAddress
        {
            get
            {
                return this.SelectedBMDSwitcherInfo != null ? this.SelectedBMDSwitcherInfo.Address : string.Empty;
            }
            set
            {
                if (this.SelectedBMDSwitcherInfo != null && this.SelectedBMDSwitcherInfo.Address != value)
                {
                    this.SelectedBMDSwitcherInfo.Address = value;

                    TryRecordCurrentItemAsDirty();
                }
            }
        }

        public ICommand AddCommand
        {
            get { return _addCommand; }
        }

        public ICommand RemoveCommand
        { get { return _removeCommand; } }

        public ICommand UseItCommand
        {
            get { return _useItCommand; }
        }

        public bool CanEditCurrent
        { get { return this.SelectedBMDSwitcherInfo != null; } }

        private void ExecuteRemove()
        {
            if (CanExecuteRemove())
            {
                var item = this.SelectedBMDSwitcherInfo;
                if (!item.IsNew())
                {
                    _removedItems.Add(item);
                }

                this.SwitcherInfos.Remove(item);
                this.SelectedBMDSwitcherInfo = null;
            }
        }

        private bool CanExecuteRemove()
        {
            return this.SelectedBMDSwitcherInfo != null;
        }

        private void ExecuteAdd()
        {
            var item = new BMDSwitcherInfo();
            this.SwitcherInfos.Add(item);
            this.SelectedBMDSwitcherInfo = item;
        }
    }
}
