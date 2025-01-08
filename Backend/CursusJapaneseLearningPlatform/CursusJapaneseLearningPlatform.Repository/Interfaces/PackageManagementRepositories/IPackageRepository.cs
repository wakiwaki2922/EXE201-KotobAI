using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.PackageManagementRepositories
{
    public interface IPackageRepository : IGenericRepository<Package>
    {
        Task<IEnumerable<Package>> GetActivePackagesAsync();

        Task<IEnumerable<Package>> GetAllPackagesAsync();
    }
}
