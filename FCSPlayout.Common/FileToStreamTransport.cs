using System;

namespace FCSPlayout.Common
{
    public class FileToStreamTransport : FileTransportBase
    {
        //private static readonly Action<double> NullProgressObserver= (d) => { };
        //private Action<double> _progressObserver;
        private IReceiveStreamManager _receiveStreamManager;

        public FileToStreamTransport(Func<string, string> receiveFilePathGenerator, bool deleteFileWhenCancelled=true, Action<double> progressObserver = null)
            :this(new ReceiveFileStreamManager(receiveFilePathGenerator, progressObserver, deleteFileWhenCancelled))
        {
        }

        public FileToStreamTransport(string receiveFilePath, bool deleteFileWhenCancelled = true, Action<double> progressObserver = null)
            : this(new ReceiveFileStreamManager(receiveFilePath, progressObserver, deleteFileWhenCancelled))
        {
        }

        public FileToStreamTransport(IReceiveStreamManager receiveStreamManager)
        {
            _receiveStreamManager = receiveStreamManager;
        }

        protected override StreamTransportBase.IDataReceiver CreateDataReceiver(string sourceFilePath)
        {
            return new DataReceiverAdapter(_receiveStreamManager, sourceFilePath);
        }
    }
}
