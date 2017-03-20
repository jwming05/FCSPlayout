using System;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class PlaylistResponseMessage : PlaylistRequestMessage
    {
        public Guid Responser { get; set; }
        public Guid ResponseId { get; set; }

        public ResponseMessageToken GetResponseToken()
        {
            var token = new ResponseMessageToken();
            token.Sender = this.Sender;
            token.RequestId = this.RequestId;
            token.Responser = this.Responser;
            token.ResponseId = this.ResponseId;
            return token;
        }
    }
}
