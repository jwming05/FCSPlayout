namespace FCSPlayout.Domain
{
    public interface IUserService
    {
        IUser CurrentUser { get; }
        bool Login(string name, string password);
    }
}
