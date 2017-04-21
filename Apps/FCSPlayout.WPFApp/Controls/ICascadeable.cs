namespace FCSPlayout.WPFApp
{
    public interface ICascadeable
    {
        void OnUpstreamChanged();
        void Init(object obj);
    }
}
