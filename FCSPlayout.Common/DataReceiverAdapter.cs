using System;
using System.IO;

namespace FCSPlayout.Common
{
    internal class DataReceiverAdapter : StreamTransportBase.IDataReceiver
    {
        private bool _cancelled;
        private Stream _receiveStream;
        private IReceiveStreamManager _receiveStreamManager;
        //private long _totalLength;
        //private long _receivedLength;
        //private Action<double> _progressObserver;

        public DataReceiverAdapter(IReceiveStreamManager receiveStreamManager, string sourceFilePath/*, Action<double> progressObserver*/)
        {
            _receiveStreamManager = receiveStreamManager;
            //_progressObserver = progressObserver;

            _receiveStream = receiveStreamManager.CreateReceiveStream(sourceFilePath);
            _cancelled = false;

            //_receivedLength = 0;
            //_totalLength = int.MaxValue;
        }

        public void Cancel()
        {
            _cancelled = true;
        }

        public void Dispose()
        {
            if (_receiveStream != null)
            {
                _receiveStreamManager.CloseReceiveStream(_receiveStream, _cancelled);
                _receiveStream = null;
            }
        }

        public void Receive(byte[] data, int offset, int count)
        {
            _receiveStreamManager.PreWrite(data, offset, count);
            _receiveStream.Write(data, offset, count);
            _receiveStreamManager.PostWrite(data, offset, count);
            //_receivedLength += count;

            //_progressObserver(((double)_receivedLength) / ((double)_totalLength));
        }

        public void SetTotalLength(long length)
        {
            _receiveStreamManager.SetTotalLength(length);
            //_totalLength = length;
        }
    }
}
