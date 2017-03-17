using FCSPlayout.WPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    public class PersistedMediaFilesViewModel: ViewModelBase
    {
        private FCSPlayout.MediaFileRepository _repository = new MediaFileRepository();

        private int _pageSize = 50;
        private readonly ObservableCollection<BindableMediaFileItem> _mediaFileItemCollection;

        public PersistedMediaFilesViewModel()
        {
            _mediaFileItemCollection = new ObservableCollection<BindableMediaFileItem>();
        }

        public ObservableCollection<BindableMediaFileItem> MediaFileItemCollection
        {
            get
            {
                return _mediaFileItemCollection;
            }
        }

        private void Load(int pageIndex)
        {
            int totalRowCount;
            _mediaFileItemCollection.Clear();
            var mediaFiles=_repository.GetMediaFiles(pageIndex * _pageSize, _pageSize, out totalRowCount);
            foreach (var entity in mediaFiles)
            {
                _mediaFileItemCollection.Add(new BindableMediaFileItem(entity));
            }
        }
    }
}
