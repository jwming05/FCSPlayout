using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public interface IPlayControlService
    {
        void SendPlaylistRequest(IPlayItem[] playItems,PlaylistRequestCategory requestCategory);
        void SendPlaylistResponse(Guid sender, Guid requestId, IPlayItem[] playItems, PlaylistRequestCategory requestCategory);
        event EventHandler<PlaylistRequestEventArgs> PlaylistRequestReceived;
        event EventHandler<PlaylistResponseEventArgs> PlaylistResponseReceived;
    }

    //public interface IRequestObject
    //{
    //    /// <summary>
    //    /// 请求方标识。
    //    /// </summary>
    //    Guid Requester { get; }

    //    /// <summary>
    //    /// 请求标识。
    //    /// </summary>
    //    Guid RequestId { get; }
    //}

    //public interface IResponseObject: IRequestObject
    //{
    //    /// <summary>
    //    /// 响应方标识。
    //    /// </summary>
    //    Guid Responser { get; }

    //    /// <summary>
    //    /// 响应标识。
    //    /// </summary>
    //    Guid ResponseId { get; }
    //}
}
