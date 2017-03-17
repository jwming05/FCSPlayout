namespace FCSPlayout.Domain
{
    public interface IChannelSwitcher
    {
        void SwitchChannelFor(IMediaSource mediaSource);
    }
}
