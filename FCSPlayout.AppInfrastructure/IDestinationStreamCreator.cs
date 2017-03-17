using FCSPlayout.Domain;
using System.IO;

namespace FCSPlayout.AppInfrastructure
{
    public interface IDestinationStreamCreator
    {
        Stream Create(string destFileName, MediaFileStorage fileStorage);
    }
}
