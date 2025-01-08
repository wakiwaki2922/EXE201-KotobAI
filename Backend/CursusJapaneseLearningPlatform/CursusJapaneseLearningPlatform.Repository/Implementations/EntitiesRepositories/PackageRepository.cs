using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using CursusJapaneseLearningPlatform.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Repository.Interfaces.PackageManagementRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Implementations.EntitiesRepositories
{
    

    // Repository/PackageRepository.cs
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        private readonly ApplicationDbContext _context;

        public PackageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
        public async Task<IEnumerable<Package>> GetActivePackagesAsync()
        {
            return await _context.Packages
                .Where(p => p.IsActive && !p.IsDelete)
                .ToListAsync();
        }

        public async Task<IEnumerable<Package>> GetAllPackagesAsync()
        {
            return await _context.Packages
                .ToListAsync();
        }
    }
}
