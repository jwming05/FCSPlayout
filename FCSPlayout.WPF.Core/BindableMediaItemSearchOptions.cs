using FCSPlayout.Domain;
using Prism.Mvvm;
using System;

namespace FCSPlayout.WPF.Core
{
    public class BindableMediaItemSearchOptions:BindableBase
    {
        private MediaItemSearchOptions _options;

        public BindableMediaItemSearchOptions()
            :this(new MediaItemSearchOptions())
        {
        }

        public BindableMediaItemSearchOptions(MediaItemSearchOptions options)
        {
            _options = options;
        }

        internal MediaItemSearchOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;

                this.RaisePropertyChanged(nameof(Title));
                this.RaisePropertyChanged(nameof(MediaFileCategoryId));
                this.RaisePropertyChanged(nameof(MediaFileChannelId));
                this.RaisePropertyChanged(nameof(MinCreationTime));
                this.RaisePropertyChanged(nameof(MaxCreationTime));
                this.RaisePropertyChanged(nameof(CurrentUserCreatedOnly));
            }
        }

        public string Title
        {
            get { return _options.Title; }
            set
            {
                _options.Title = value;
                this.RaisePropertyChanged(nameof(Title));
            }
        }
        public Guid MediaFileCategoryId
        {
            get { return _options.MediaFileCategoryId ?? Guid.Empty; }
            set
            {
                _options.MediaFileCategoryId = value==Guid.Empty ? (Guid?)null : value;
                this.RaisePropertyChanged(nameof(MediaFileCategoryId));
            }
        }

        public Guid MediaFileChannelId
        {
            get { return _options.MediaFileChannelId ?? Guid.Empty; }
            set
            {
                _options.MediaFileChannelId = value == Guid.Empty ? (Guid?)null : value;
                this.RaisePropertyChanged(nameof(MediaFileChannelId));
            }
        }

        public DateTime? MinCreationTime
        {
            get { return _options.MinCreationTime; }
            set
            {
                _options.MinCreationTime = value;
                this.RaisePropertyChanged(nameof(MinCreationTime));
            }
        }
        public DateTime? MaxCreationTime
        {
            get { return _options.MaxCreationTime; }
            set
            {
                _options.MaxCreationTime = value;
                this.RaisePropertyChanged(nameof(MaxCreationTime));
            }
        }
        public bool CurrentUserCreatedOnly
        {
            get { return _options.CurrentUserCreatedOnly; }
            set
            {
                _options.CurrentUserCreatedOnly = value;
                this.RaisePropertyChanged(nameof(CurrentUserCreatedOnly));
            }
        }
    }
}
