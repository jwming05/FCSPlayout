using System;

namespace FCSPlayout.Common
{
    public class StreamToStreamTransport : StreamTransportBase
    {
        
        //private static readonly Action<double> NullProgressObserver = (d) => { };
        //private Action<double> _progressObserver;
        private IReceiveStreamManager _receiveStreamManager;

        public StreamToStreamTransport(Func<string, string> receiveFilePathGenerator, bool deleteFileWhenCancelled = true, Action<double> progressObserver = null)
            :this(new ReceiveFileStreamManager(receiveFilePathGenerator, progressObserver, deleteFileWhenCancelled))
        {
        }

        public StreamToStreamTransport(string receiveFilePath, bool deleteFileWhenCancelled = true, Action<double> progressObserver = null)
            : this(new ReceiveFileStreamManager(receiveFilePath, progressObserver, deleteFileWhenCancelled))
        {
        }

        public StreamToStreamTransport(IReceiveStreamManager receiveStreamManager)
        {
            _receiveStreamManager = receiveStreamManager;
            //_progressObserver = progressObserver ?? NullProgressObserver;
        }

        protected override StreamTransportBase.IDataReceiver CreateDataReceiver()
        {
            return new DataReceiverAdapter(_receiveStreamManager, null);
        }
    }
}
