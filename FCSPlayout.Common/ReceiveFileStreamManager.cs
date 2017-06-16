using System;
using System.IO;

namespace FCSPlayout.Common
{
    public class ReceiveFileStreamManager : IReceiveStreamManager
    {
        private string _receiveFilePath;
        private FileStream _receiveStream;
        private bool _deleteFileWhenCancelled;
        private Func<string, string> _receiveFilePathGenerator;
        private Action<double> _progressObserver;
        private long _totalLength;
        private long _receivedLength;

        public ReceiveFileStreamManager(Func<string, string> receiveFilePathGenerator, Action<double> progressObserver=null, bool deleteFileWhenCancelled = true)
        {
            _receiveFilePathGenerator = receiveFilePathGenerator;
            _progressObserver = progressObserver;
            _deleteFileWhenCancelled = deleteFileWhenCancelled;

            _totalLength = _receivedLength = 0;
        }

        public ReceiveFileStreamManager(string receiveFilePath, Action<double> progressObserver = null, bool deleteFileWhenCancelled = true):
            this((s)=>receiveFilePath, progressObserver, deleteFileWhenCancelled)
        {
        }

        public void CloseReceiveStream(Stream receiveStream, bool cancelled)
        {
            if (_receiveStream == receiveStream)
            {
                _receiveStream.Close();
                if (cancelled && _deleteFileWhenCancelled && File.Exists(_receiveFilePath))
                {
                    DeleteReceiveFile();
                }
            }
        }

        private void DeleteReceiveFile()
        {
            try
            {
                File.Delete(_receiveFilePath);
            }
            catch
            {

            }
        }

        public Stream CreateReceiveStream(string sourceFilePath)
        {
            _receiveFilePath = _receiveFilePathGenerator(sourceFilePath);
            _receiveStream = new FileStream(_receiveFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            return _receiveStream;
        }

        public virtual void PreWrite(byte[] data, int offset, int count)
        {
        }

        public void SetTotalLength(long length)
        {
            _totalLength = length;
        }

        public virtual void PostWrite(byte[] data, int offset, int count)
        {
            if (_progressObserver != null && _totalLength>0)
            {
                _receivedLength += count;
                _progressObserver(((double)_receivedLength)/((double)_totalLength));
            }
        }
    }
}
