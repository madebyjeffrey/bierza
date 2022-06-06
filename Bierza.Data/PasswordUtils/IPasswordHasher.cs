namespace Bierza.Data.PasswordUtils;

/* Based on details from https://medium.com/dealeron-dev/storing-passwords-in-net-core-3de29a3da4d2 */
public interface IPasswordHasher
{
    string Hash(string password);

    (bool Verified, bool NeedsUpgrade) Check(string hash, string password);
}