using System;

namespace FCSPlayout.Domain
{
    public class MediaItemSearchOptions
    {
        public string Title { get; set; }
        public Guid? MediaFileCategoryId { get; set; }
        public Guid? MediaFileChannelId { get; set; }

        public DateTime? MinCreationTime { get; set; }
        public DateTime? MaxCreationTime { get; set; }
        public bool CurrentUserCreatedOnly { get; set; }
    }
}
