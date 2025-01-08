using CursusJapaneseLearningPlatform.Service.BusinessModels.RoleModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.RoleModels.Responses;

namespace CursusJapaneseLearningPlatform.Service.Interfaces;
public interface IRoleService
{
    Task<bool> CreateRole(RoleRequestModel request, CancellationToken cancellationToken);
    Task<RoleResponseModel> GetRoleById(Guid request, CancellationToken cancellationToken);
    Task InitializeRolesAsync();
    Task<List<RoleResponseModel>> GetRolesAsync(CancellationToken cancellationToken);
}
