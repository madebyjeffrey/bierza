namespace Bierza.Business.UserManagement;

public class UserDataException : Exception
{
    public UserDataException() : base()
    {
    }

    public UserDataException(string message) : base(message)
    {
    }

    public UserDataException(string message, Exception inner) : base(message, inner)
    {
    }
}