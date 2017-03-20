using System;
using System.Net;

namespace NetworkLib
{
    public class NetDataReceivedEventArgs : EventArgs
    {
        public IPEndPoint RemoteEndPoint { get; private set; }
        public byte[] Data { get; private set; }

        public NetDataReceivedEventArgs(byte[] data, IPEndPoint remoteEndPoint)
        {
            this.Data = data;
            this.RemoteEndPoint = remoteEndPoint;
        }
    }
}
