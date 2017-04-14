using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PixelFormatViewer
{
    public class MainViewModel:BindableBase
    {
        private List<PixelFormat> _pixelFormats;
        private PixelFormat _selectedFormat;

        private IPixelFormatService _pixelFormatService;
        private ObservableCollection<PixelFormatChannelMask> _channelMaskCollection;
        private ListCollectionView _channelMaskView;

        private ObservableCollection<byte> _maskCollection;
        private ListCollectionView _maskView;

        public MainViewModel(IPixelFormatService pixelFormatService)
        {
            _pixelFormatService = pixelFormatService;

            _channelMaskCollection = new ObservableCollection<PixelFormatChannelMask>();
            _channelMaskView = new ListCollectionView(_channelMaskCollection);
            _channelMaskView.CurrentChanged += ChannelMaskView_CurrentChanged;

            _maskCollection = new ObservableCollection<byte>();
            _maskView = new ListCollectionView(_maskCollection);
            _maskView.CurrentChanged += MaskView_CurrentChanged;

            var task = LoadAllPixelFormats();
        }

        public PixelFormat SelectedFormat
        {
            get { return _selectedFormat; }
            set
            {
                base.SetProperty(ref _selectedFormat, value, OnSetSelectedFormat);
            }
        }

        public List<PixelFormat> PixelFormats
        {
            get
            {
                return _pixelFormats;
            }

            set
            {
                base.SetProperty(ref _pixelFormats, value, OnSetPixelFormats);
            }
        }

        public ListCollectionView ChannelMaskView
        {
            get
            {
                return _channelMaskView;
            }
        }

        public ListCollectionView MaskView
        {
            get
            {
                return _maskView;
            }
        }

        private void OnSetSelectedFormat()
        {
            _channelMaskCollection.Clear();

            try
            {
                if (_selectedFormat != null && _selectedFormat.Masks.Any())
                {
                    _channelMaskCollection.AddRange(_selectedFormat.Masks);
                    _channelMaskView.MoveCurrentToFirst();
                }
            }
            catch
            {

            }
        }

        private void OnSetPixelFormats()
        {
            if (_pixelFormats != null)
            {
                this.SelectedFormat = _pixelFormats.FirstOrDefault();
            }
        }
        
        private async Task LoadAllPixelFormats()
        {
            this.PixelFormats = new List<PixelFormat>(await _pixelFormatService.GetAllPixelFormatsAsync());
        }

        private void MaskView_CurrentChanged(object sender, System.EventArgs e)
        {
        }

        private void ChannelMaskView_CurrentChanged(object sender, System.EventArgs e)
        {
            _maskCollection.Clear();
            if (_channelMaskView.CurrentItem != null)
            {
                try
                {
                    PixelFormatChannelMask current = (PixelFormatChannelMask)_channelMaskView.CurrentItem;

                    if (current.Mask != null && current.Mask.Any())
                    {
                        _maskCollection.AddRange(current.Mask);
                        _maskView.MoveCurrentToFirst();
                    }
                }
                catch
                {
                }
            }
        }
    }
}
