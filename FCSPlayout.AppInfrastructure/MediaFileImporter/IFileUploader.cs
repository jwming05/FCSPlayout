using System;

namespace FCSPlayout.AppInfrastructure
{
    public interface IFileUploader
    {
        void UploadFile(string filePath, string destFileName, Action<int, object> progressReporter);
    }
}
