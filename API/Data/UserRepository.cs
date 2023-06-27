using API.Entitites;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<AppUser> GetUserById(int id)
        {
            return await _dataContext.Users.Include(b => b.Photos).SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AppUser> GetUserByUserName(string username)
        {
            return await _dataContext.Users.Include(b => b.Photos).SingleOrDefaultAsync(a => a.UserName.Equals(username));
        }

        public async Task<IEnumerable<AppUser>> GetUsers()
        {
            return await _dataContext.Users.Include(b => b.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _dataContext.Entry(user).State = EntityState.Modified;
        }
    }
}
