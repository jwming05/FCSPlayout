using System;
using System.Collections.Generic;
using System.Net;

namespace NetworkLib
{
    public class MessageSegmentParser
    {
        private SortedList<int, MessageSegment> _segments = new SortedList<int, MessageSegment>();

        public MessageSegmentParser(IPEndPoint remoteEndPoint, Guid messageId, int totalLength)
        {
            this.RemoteEndPoint = remoteEndPoint;
            this.MessageId = messageId;
            this.TotalLength = totalLength;
            this.ReceivedLength = 0;
        }

        public void AddSegment(MessageSegment segment)
        {
            if (this.MessageId != segment.MessageId || this.TotalLength != segment.TotalLength)
            {
                throw new InvalidOperationException();
            }

            if (!_segments.ContainsKey(segment.SegmentNumber))
            {
                _segments.Add(segment.SegmentNumber, segment);
                this.ReceivedLength += segment.Length;

                if (this.ReceivedLength == this.TotalLength)
                {
                    var result = new byte[this.TotalLength];

                    int offset = 0;
                    for (int i = 0; i < _segments.Count; i++)
                    {
                        var current = _segments[i];

                        if(offset != current.Offset)
                        {
                            throw new InvalidOperationException();
                        }
                        Array.Copy(current.Data, 0, result, offset, current.Length);

                        offset += current.Length;
                    }

                    if(offset != this.TotalLength)
                    {
                        throw new InvalidOperationException();
                    }
                    OnMessageReceived(result);
                }
            }
        }

        public event EventHandler<NetDataReceivedEventArgs> DataReceived;

        public IPEndPoint RemoteEndPoint { get; private set; }

        private void OnMessageReceived(byte[] data)
        {
            if (DataReceived != null)
            {
                DataReceived(this, new NetDataReceivedEventArgs(data, this.RemoteEndPoint));
            }
        }

        public Guid MessageId { get; private set; }

        public int TotalLength { get; private set; }

        private int ReceivedLength { get; set; }
    }
}
