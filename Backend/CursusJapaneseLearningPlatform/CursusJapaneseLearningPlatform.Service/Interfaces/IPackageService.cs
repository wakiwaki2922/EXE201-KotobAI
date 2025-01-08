using CursusJapaneseLearningPlatform.Service.BusinessModels.PackageModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Interfaces
{
    public interface IPackageService
    {
        Task<BaseResponseModel<PackageResponseModel>> CreatePackage(PackageRequestModel request);
        Task<BaseResponseModel<IEnumerable<PackageResponseModel>>> GetActivePackages();
        Task<BaseResponseModel<PackageResponseModel>> UpdatePackage(Guid packageId, PackageRequestModel request);
        Task<BaseResponseModel<bool>> DeletePackage(Guid packageId);
        Task<BaseResponseModel<PackageResponseModel>> GetPackageById(Guid packageId);
        Task<BaseResponseModel<IEnumerable<PackageResponseModel>>> GetAllPackages();
    }

}
