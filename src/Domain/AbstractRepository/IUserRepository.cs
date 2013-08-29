using Domain.Users;
using Infrastructure.DomainBase;

namespace Domain.AbstractRepository
{
    public interface IUserRepository : IRepository<User>
    {
        bool Authenticate(string email, string password);
        bool IsUserEmailUnique(string email);
        void Create(User user, string password);
        void ChangePassword(User user, string password);
        string GetHashForNewPasswordRequest(User user);
        bool IsHashForNewPasswordRequestValid(User user, string hash);
    }
}