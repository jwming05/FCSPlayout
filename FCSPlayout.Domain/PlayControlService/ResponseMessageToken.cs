using System;

namespace FCSPlayout.Domain
{
    public struct ResponseMessageToken : IEquatable<ResponseMessageToken>
    {
        public Guid Sender { get; set; }

        public Guid RequestId { get; set; }

        public Guid Responser { get; set; }
        public Guid ResponseId { get; set; }

        public bool Equals(ResponseMessageToken other)
        {
            return this.Sender == other.Sender && this.RequestId == other.RequestId &&
                this.Responser == other.Responser && this.ResponseId == other.ResponseId;
        }

        public override bool Equals(object obj)
        {
            return base.Equals((ResponseMessageToken)obj);
        }

        public override int GetHashCode()
        {
            return this.Sender.GetHashCode() ^ this.RequestId.GetHashCode() ^
                this.Responser.GetHashCode() ^ this.ResponseId.GetHashCode();
        }
    }
}
