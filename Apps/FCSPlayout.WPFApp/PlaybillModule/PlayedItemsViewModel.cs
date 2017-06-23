using FCSPlayout.WPF.Core;
using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;

namespace FCSPlayout.WPFApp
{
    public class PlayedItemsViewModel: ViewModelBase
    {
        private ObservableCollection<BindablePlayItem> _playedCollection;
        private BindablePlayItem _selectedPlayItem;

        public PlayedItemsViewModel([Dependency("playedCollection")]ObservableCollection<BindablePlayItem> playedCollection)
        {
            _playedCollection = playedCollection;
        }

        public ObservableCollection<BindablePlayItem> PlayItemCollection
        {
            get { return _playedCollection; }
        }

        public BindablePlayItem SelectedPlayItem
        {
            get { return _selectedPlayItem; }
            set
            {
                _selectedPlayItem = value;
                this.RaisePropertyChanged(nameof(this.SelectedPlayItem));
            }
        }
    }
}