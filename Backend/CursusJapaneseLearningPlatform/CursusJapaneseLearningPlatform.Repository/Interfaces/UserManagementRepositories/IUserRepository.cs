using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.UserManagementRepositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<IEnumerable<User>> GetUsersByFullNameAsync(string fullName);
    Task<string> GetUserEmailByIdAsync(string userId);
    Task UpdateAsync(User user);
    Task UpdateUserAsync(User user);
    Task<string?> GetUserNameByIdAsync(object id);
    Task<Dictionary<string, string>> GetFullNamesByIdsAsync(IEnumerable<string> userIds);
}
