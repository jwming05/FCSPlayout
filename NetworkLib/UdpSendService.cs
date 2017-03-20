using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkLib
{
    public class UdpSendService : IDisposable
    {
        private Task _sendTask;
        private BlockingCollection<byte[]> _sendQueue;
        private CancellationTokenSource _cts;
        private UdpClient _udpClient;
        private IPEndPoint _remoteEndPoint;

        private int _maxSegmentLength = 900;

        public UdpSendService(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, int boundedCapacity)
        {
            _udpClient = new UdpClient(localEndPoint);
            //_udpClient.DontFragment = true;
            _udpClient.EnableBroadcast = true;

            _sendQueue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>(), boundedCapacity);
            _remoteEndPoint = remoteEndPoint;
        }

        public UdpSendService(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
            : this(localEndPoint, remoteEndPoint, 100)
        {

        }

        public void Send(byte[] data)
        {
            _sendQueue.Add(data);
        }

        public void Start()
        {
            if (_sendQueue != null && _sendTask == null)
            {
                _cts = new CancellationTokenSource();
                _sendTask = new Task(SendProc);
                _sendTask.Start();
            }

        }

        public void Stop()
        {
            if (_sendTask != null)
            {
                _cts.Cancel();
                _sendTask.Wait();

                _cts.Dispose();
                _cts = null;
                _sendTask.Dispose();
                _sendTask = null;

                byte[] temp;
                while (_sendQueue.TryTake(out temp)) ;
            }
        }

        private void SendProc()
        {
            while (!_cts.IsCancellationRequested)
            {
                byte[] data = null;
                try
                {
                    data = _sendQueue.Take(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (data != null)
                {
                    SendData(data);
                }

            }
        }

        private void SendData(byte[] data)
        {
            Guid messageId = Guid.NewGuid();
            int segmentId = 0;
            int totalLength = data.Length;
            int segmentOffset = 0;

            while (segmentOffset < totalLength)
            {
                var nextOffset = segmentOffset + _maxSegmentLength;
                if (nextOffset > totalLength)
                {
                    nextOffset = totalLength;
                }

                int length = nextOffset - segmentOffset;
                byte[] buffer = new byte[length + 32];

                Array.Copy(messageId.ToByteArray(), 0, buffer, 0, 16);
                Array.Copy(BitConverter.GetBytes(segmentId), 0, buffer, 16, 4);
                Array.Copy(BitConverter.GetBytes(totalLength), 0, buffer, 20, 4);
                Array.Copy(BitConverter.GetBytes(segmentOffset), 0, buffer, 24, 4);
                Array.Copy(BitConverter.GetBytes(length), 0, buffer, 28, 4);

                Array.Copy(data, segmentOffset, buffer, 32, length);

                _udpClient.Send(buffer, buffer.Length, _remoteEndPoint);

                segmentId++;
                segmentOffset = nextOffset;

                Thread.Sleep(0);
            }
        }


        public void Dispose()
        {
            this.Stop();

            if (_sendQueue != null)
            {
                _sendQueue.Dispose();
                _sendQueue = null;
            }
        }
    }
}
