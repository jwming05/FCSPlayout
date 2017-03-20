using System;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class PlaylistRequestMessage
    {
        public Guid Sender { get; set; }

        public Guid RequestId { get; set; }

        public IPlayItem[] PlayItems { get; set; }

        public PlaylistRequestCategory Category { get; set; }

        public RequestMessageToken GetRequestToken()
        {
            var token = new RequestMessageToken();
            token.Sender = this.Sender;
            token.RequestId = this.RequestId;
            return token;
        }
    }
}
