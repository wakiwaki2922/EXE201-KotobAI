using Microsoft.EntityFrameworkCore;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.UserManagementRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Implementations.UserManagementRepositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<IEnumerable<User>> GetUsersByFullNameAsync(string fullName)
    {
        var searchTerm = fullName.Trim().ToLower();
        var users = await _context.Users
            .Where(u => u.FullName.ToLower().Contains(searchTerm) && u.IsDelete == false && u.IsActive == true)
            .ToListAsync();
        return users;
    }


    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<string?> GetUserNameByIdAsync(object id)
    {
        var user = await _context.Set<User>().FindAsync(id);
        if (user == null)
        {
            return null;
        }
        return user.FullName;
    }

    public async Task<Dictionary<string, string>> GetFullNamesByIdsAsync(IEnumerable<string> userIds)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserEmailByIdAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
