using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("ChannelInfos")]
    [Serializable]
    public class ChannelInfo : MediaSourceEntity, IEquatable<ChannelInfo>
    {
        [System.Xml.Serialization.XmlIgnore]
        public bool Special { get; set; }

        public bool IsNew()
        {
            return this.Id == Guid.Empty; // int.MinValue;
        }

        public override string ToString()
        {
            return this.Title ?? string.Empty;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ChannelInfo);
        }

        public override int GetHashCode()
        {
            var temp = string.IsNullOrEmpty(this.Title) ? 0 : this.Title.GetHashCode();
            return this.Id.GetHashCode() ^ this.Special.GetHashCode() ^ temp;
        }

        public bool Equals(ChannelInfo other)
        {
            if (other == null) return false;
            return this.Id == other.Id && this.Special == other.Special && string.Equals(this.Title, other.Title);
        }
    }
}
