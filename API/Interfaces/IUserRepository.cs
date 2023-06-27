using API.Entitites;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsers();

        Task<AppUser> GetUserById(int id);

        Task<AppUser> GetUserByUserName(string username);
    }
}
