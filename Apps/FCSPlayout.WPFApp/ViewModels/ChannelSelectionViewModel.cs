using FCSPlayout.Entities;
using System.Collections.ObjectModel;

namespace FCSPlayout.WPFApp.ViewModels
{
    public class ChannelSelectionViewModel
    {
        public ChannelSelectionViewModel()
        {
            this.ChannelInfos = new ObservableCollection<ChannelInfo>(PlayoutRepository.GetChannelInfos());
        }

        public ObservableCollection<ChannelInfo> ChannelInfos { get; private set; }
    }
}
