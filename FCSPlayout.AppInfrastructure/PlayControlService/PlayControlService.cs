using FCSPlayout.Domain;
using NetworkLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace FCSPlayout.AppInfrastructure
{
    public class PlayControlService:IPlayControlService
    {
        private static PlayControlService _current=new PlayControlService();
        private UdpSendService _sendService;
        private UdpReceiveService _receiveService;
        private MessageParser _messageParser;
        private Queue<ResponseMessageToken> _responseTokens = new Queue<ResponseMessageToken>();
        private Queue<RequestMessageToken> _requestTokens = new Queue<RequestMessageToken>();
        public static PlayControlService Current
        {
            get
            {
                return _current;
            }

            set
            {
                _current = value;
            }
        }

        public event EventHandler<PlaylistRequestEventArgs> PlaylistRequestReceived;
        public event EventHandler<PlaylistResponseEventArgs> PlaylistResponseReceived;
        public PlayControlService()
        {
            this.ServiceId = Guid.NewGuid();
            var localSendEP = new IPEndPoint(IPAddress.Any,0);

            var localReceiveEP = new IPEndPoint(IPAddress.Any, 
                int.Parse(ConfigurationManager.AppSettings["LocalReceiveEndPoint.Port"]));
            var remoteReceiveEP= new IPEndPoint(IPAddress.Broadcast, 
                int.Parse(ConfigurationManager.AppSettings["RemoteReceiveEndPoint.Port"]));

            _sendService = new UdpSendService(localSendEP, remoteReceiveEP);
            _receiveService = new UdpReceiveService(localReceiveEP);
            _receiveService.DataReceived += _receiveService_DataReceived;
            _messageParser = new MessageParser();
            _messageParser.DataReceived += _messageParser_DataReceived;

            _sendService.Start();
            _receiveService.Start();
        }

        private void _messageParser_DataReceived(object sender, NetDataReceivedEventArgs e)
        {
            PlaylistRequestMessage requestMessage = null;
            try
            {
                requestMessage = Deserialize<PlaylistRequestMessage>(e.Data);
            }
            catch
            {
                requestMessage = null;
            }

            
            if(requestMessage!=null)
            {
                PlaylistResponseMessage responseMessage = requestMessage as PlaylistResponseMessage;
                if (responseMessage != null)
                {
                    if(responseMessage.ResponseId != this.ServiceId && responseMessage.RequestId == this.ServiceId)
                    {
                        if (!HasReceived(responseMessage))
                        {
                            OnResponseReceived(responseMessage);
                        }
                    }
                }
                else
                {
                    if (requestMessage.Sender != this.ServiceId)
                    {
                        if (!HasReceived(requestMessage))
                        {
                            OnRequestReceived(requestMessage);
                        }
                    }
                }
            }
        }

        private void OnRequestReceived(PlaylistRequestMessage requestMessage)
        {
            if (this.PlaylistRequestReceived != null)
            {
                this.PlaylistRequestReceived(this, new PlaylistRequestEventArgs(requestMessage));
            }
        }

        private void OnResponseReceived(PlaylistResponseMessage responseMessage)
        {
            if (this.PlaylistResponseReceived != null)
            {
                this.PlaylistResponseReceived(this, new PlaylistResponseEventArgs(responseMessage));
            }
        }

        private bool HasReceived(PlaylistRequestMessage requestMessage)
        {
            bool result;
            var token = requestMessage.GetRequestToken();
            if (_requestTokens.Contains(token))
            {
                result = true;
            }
            else
            {
                _requestTokens.Enqueue(token);
                result = false;
            }

            if (_requestTokens.Count > 100)
            {
                while (_requestTokens.Count > 50)
                {
                    _requestTokens.Dequeue();
                }
            }

            return result;
        }

        private bool HasReceived(PlaylistResponseMessage responseMessage)
        {
            bool result;
            var token = responseMessage.GetResponseToken();
            if (_responseTokens.Contains(token))
            {
                result = true;
            }
            else
            {
                _responseTokens.Enqueue(token);
                result = false;
            }

            if (_responseTokens.Count > 100)
            {
                while (_responseTokens.Count > 50)
                {
                    _responseTokens.Dequeue();
                }
            }

            return result;
        }

        private void OnPlaylistSyncRequestReceived(PlaylistRequestMessage message)
        {

            //throw new NotImplementedException();
        }

        private void _receiveService_DataReceived(object sender, NetDataReceivedEventArgs e)
        {
            _messageParser.AddData(e.RemoteEndPoint, e.Data);
        }

        public void SendPlaylistRequest(IPlayItem[] playItems, PlaylistRequestCategory requestCategory)
        {
            var requestId = Guid.NewGuid();

            var message = new PlaylistRequestMessage()
            {
                Category = requestCategory,
                PlayItems = playItems,
                RequestId = requestId,
                Sender = this.ServiceId
            };

            Send(message);
        }

        public void SendPlaylistResponse(Guid sender, Guid requestId, IPlayItem[] playItems, PlaylistRequestCategory requestCategory)
        {
            var responseId = Guid.NewGuid();

            var message = new PlaylistResponseMessage()
            {
                Category = requestCategory,
                PlayItems = playItems,
                ResponseId = responseId,
                Responser = this.ServiceId,
                RequestId=requestId,
                Sender=sender
            };

            Send(message);
        }

        private void Send(PlaylistRequestMessage message)
        {
            byte[] data = Serialize(message);
            _sendService.Send(data);
        }

        private byte[] Serialize(object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using(var ms=new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private T Deserialize<T>(byte[] data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using(var ms=new MemoryStream(data))
            {
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)formatter.Deserialize(ms);
            }
        }

        

        public Guid ServiceId
        {
            get; private set;
        }
    }
}
