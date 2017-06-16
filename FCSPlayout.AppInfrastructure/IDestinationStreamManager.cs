using FCSPlayout.Domain;
using System.IO;

namespace FCSPlayout.AppInfrastructure
{
    public interface IDestinationStreamManager
    {
        Stream Create(string destFileName, MediaFileStorage fileStorage);
        void Delete(string destFileName, MediaFileStorage fileStorage);
    }
}
