using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CursusJapaneseLearningPlatform.Service.BusinessModels.RoleModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.RoleModels.Responses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using CursusJapaneseLearningPlatform.Repository.Entities;

namespace CursusJapaneseLearningPlatform.Service.Implementations;
public sealed class RoleService : IRoleService
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;

    public RoleService(RoleManager<Role> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<bool> CreateRole(RoleRequestModel request, CancellationToken cancellationToken)
    {
        try
        {
            var newRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            // Attempt to create the role using RoleManager
            var result = await _roleManager.CreateAsync(newRole);

            // Check if the role creation succeeded
            if (!result.Succeeded)
            {
                throw new CustomException(
                    StatusCodes.Status409Conflict,
                    ResponseCodeConstants.BADREQUEST,
                    ResponseMessages.DUPLICATE
                );
            }

            return result.Succeeded;
        }
        catch (CustomException)
        {
            throw; // Re-throw custom exceptions to maintain the original context
        }
        catch (Exception)
        {
            // Handle any unexpected errors
            throw new CustomException(
                StatusCodes.Status500InternalServerError,
                ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                ResponseMessages.INTERNAL_SERVER_ERROR
            );
        }
    }

    public async Task<RoleResponseModel> GetRoleById(Guid request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleManager.Roles
            .FirstOrDefaultAsync(rl => rl.Id == request, cancellationToken);

            if (role == null)
            {
                throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, ResponseMessages.NOT_FOUND.Replace("{0}", "vai trò"));
            }

            return _mapper.Map<RoleResponseModel>(role);
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, ResponseMessages.INTERNAL_SERVER_ERROR);
        }
    }

    public Task<List<RoleResponseModel>> GetRolesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var roles = _roleManager.Roles.ToList();
            return Task.FromResult(_mapper.Map<List<RoleResponseModel>>(roles));
        }
        catch (CustomException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new CustomException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, ResponseMessages.INTERNAL_SERVER_ERROR);
        }
    }

    public async Task InitializeRolesAsync()
    {
        await CreateRoleIfNotExists("Admin", "Users who have logged in using CursusA.I admin web application");
        await CreateRoleIfNotExists("Learner", "Users who logged in using CursusA.I mobile or web application");
        await CreateRoleIfNotExists("Guest", "Users who have not logged in using CursusA.I mobile or web application");
        await CreateRoleIfNotExists("System", "CursusA.I’s system");
    }

    private async Task CreateRoleIfNotExists(string name, string description)
    {
        // Kiểm tra xem vai trò đã tồn tại chưa
        var existingRole = await _roleManager.FindByNameAsync(name);
        if (existingRole == null)
        {
            var role = new Role
            {
                Name = name,
                Description = description
            };
            await _roleManager.CreateAsync(role);
        }
    }
}
