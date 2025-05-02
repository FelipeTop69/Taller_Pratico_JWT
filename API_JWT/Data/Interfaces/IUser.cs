using Entity.Models;

namespace Data.Interfaces
{
    public interface IUser
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
