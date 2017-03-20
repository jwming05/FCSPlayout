using System;
using System.Net;
using System.Collections.Generic;

namespace NetworkLib
{
    public class MessageParser
    {
        private Dictionary<EndPointInfo, Dictionary<Guid, MessageSegmentParser>> _parseData = 
            new Dictionary<EndPointInfo, Dictionary<Guid, MessageSegmentParser>>();
        public void AddData(IPEndPoint remoteEndPoint, byte[] data)
        {
            lock (_parseData)
            {
                var segment = MessageSegment.Create(data);
                if (segment == null) return;

                var endPointInfo = new EndPointInfo(remoteEndPoint);
                Dictionary<Guid, MessageSegmentParser> parsers;
                if (_parseData.ContainsKey(endPointInfo))
                {
                    parsers = _parseData[endPointInfo];
                }
                else
                {
                    parsers = new Dictionary<Guid, MessageSegmentParser>();
                    _parseData.Add(endPointInfo, parsers);
                }


                MessageSegmentParser segmentParser;
                if (parsers.ContainsKey(segment.MessageId))
                {
                    segmentParser = parsers[segment.MessageId];
                }
                else
                {
                    segmentParser = CreateSegmentParser(remoteEndPoint, segment);
                    parsers.Add(segment.MessageId, segmentParser);
                }

                try
                {
                    segmentParser.AddSegment(segment);

                    // TODO: 发送片断应答。
                }
                catch
                {
                    RemoveSegmentParser(segmentParser);
                }
            }
        }

        private MessageSegmentParser CreateSegmentParser(IPEndPoint remoteEndPoint, MessageSegment segment)
        {
            MessageSegmentParser segmentParser = new MessageSegmentParser(remoteEndPoint, segment.MessageId, segment.TotalLength);
            segmentParser.DataReceived += SegmentParser_DataReceived;
            return segmentParser;
        }


        public event EventHandler<NetDataReceivedEventArgs> DataReceived;

        private void OnMessageReceived(NetDataReceivedEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        private void RemoveSegmentParser(MessageSegmentParser parser)
        {
            parser.DataReceived -= SegmentParser_DataReceived;
            var endPointInfo = new EndPointInfo(parser.RemoteEndPoint);
            if (_parseData.ContainsKey(endPointInfo))
            {
                if (_parseData[endPointInfo].Remove(parser.MessageId))
                {
                    if (_parseData[endPointInfo].Count == 0)
                    {
                        _parseData.Remove(endPointInfo);
                    }
                }
            }
        }

        private void SegmentParser_DataReceived(object sender, NetDataReceivedEventArgs e)
        {
            MessageSegmentParser parser = (MessageSegmentParser)sender;
            RemoveSegmentParser(parser);
            OnMessageReceived(e);
        }
    }
}
