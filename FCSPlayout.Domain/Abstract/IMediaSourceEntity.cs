using System;

namespace FCSPlayout.Domain
{
    public interface IMediaSourceEntity
    {
        Guid Id { get; set; }
        string Title { get; set; }
    }
}
