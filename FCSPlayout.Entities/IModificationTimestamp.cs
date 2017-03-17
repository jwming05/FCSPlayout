using System;

namespace FCSPlayout.Entities
{
    public interface IModificationTimestamp : ICreationTimestamp
    {
        DateTime ModificationTime { get; set; }
    }
}