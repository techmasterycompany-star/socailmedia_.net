namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }

}
