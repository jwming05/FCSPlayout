namespace FCSPlayout.AppInfrastructure
{
    public interface IBackgroundWorkContext
    {
        void SetProgress(int progress);
        void SetProgress(int progress, object state);
    }
}