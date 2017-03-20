using System;

namespace FCSPlayout.Domain
{
    public struct RequestMessageToken : IEquatable<RequestMessageToken>
    {
        public Guid Sender { get; set; }

        public Guid RequestId { get; set; }

        public bool Equals(RequestMessageToken other)
        {
            return this.Sender == other.Sender && this.RequestId == other.RequestId;
        }

        public override bool Equals(object obj)
        {
            return base.Equals((RequestMessageToken)obj);
        }

        public override int GetHashCode()
        {
            return this.Sender.GetHashCode() ^ this.RequestId.GetHashCode();
        }
    }
}
