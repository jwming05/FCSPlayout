using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    public interface IUploadProgressFeedback
    {
        void Open();
        void Close();
        void Report(int progress, MediaFileStorage locationCategory);
        void Reset();
    }

    class NullUploadProgressFeedback : IUploadProgressFeedback
    {
        private static readonly NullUploadProgressFeedback _instance = new NullUploadProgressFeedback();

        internal static NullUploadProgressFeedback Instance
        {
            get
            {
                return _instance;
            }
        }
        private NullUploadProgressFeedback()
        {
        }

        public void Close()
        {
        }

        public void Open()
        {
        }

        public void Report(int progress, MediaFileStorage locationCategory)
        {
        }

        public void Reset()
        {
        }
    }
}
