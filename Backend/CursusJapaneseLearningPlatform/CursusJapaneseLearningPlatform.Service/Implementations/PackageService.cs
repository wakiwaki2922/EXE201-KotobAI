using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using Microsoft.AspNetCore.Http;
using CursusJapaneseLearningPlatform.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using CursusJapaneseLearningPlatform.Service.BusinessModels.PackageModels;
using CursusJapaneseLearningPlatform.Repository.Migrations;

namespace CursusJapaneseLearningPlatform.Service.Implementations
{

    // Services/PackageService.cs
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<PackageResponseModel>> CreatePackage(PackageRequestModel request)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var package = new Repository.Entities.Package
                {
                    PlanType = request.PlanType,
                    PlanName = request.PlanName,
                    Period = request.Period,
                    Price = request.Price,
                    IsActive = request.Status,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow,
                    CreatedBy = "Admin",
                    LastUpdatedBy = "Admin",
                    IsDelete = false
                };

                var createdPackage = await _unitOfWork.PackageRepository.AddAsync(package);
                await _unitOfWork.SaveAsync();

                var responseModel = new PackageResponseModel(createdPackage);
                _unitOfWork.CommitTransaction();

                return BaseResponseModel<PackageResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception)
            {
                _unitOfWork.RollBack();
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task<BaseResponseModel<IEnumerable<PackageResponseModel>>> GetActivePackages()
        {
            try
            {
                var packages = await _unitOfWork.PackageRepository.GetActivePackagesAsync();
                var responseModels = packages.Select(p => new PackageResponseModel(p));
                return BaseResponseModel<IEnumerable<PackageResponseModel>>.OkResponseModel(responseModels);
            }
            catch (Exception)
            {
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
        }
        public async Task<BaseResponseModel<PackageResponseModel>> UpdatePackage(Guid packageId, PackageRequestModel request)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var package = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
                if (package == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Package not found");
                }

                package.PlanType = request.PlanType;
                package.PlanName = request.PlanName;
                package.Period = request.Period;
                package.Price = request.Price;
                package.IsActive = request.Status;
                package.LastUpdatedTime = DateTime.UtcNow;
                package.LastUpdatedBy = "Admin";

                await _unitOfWork.PackageRepository.UpdateAsync(package);
                await _unitOfWork.SaveAsync();

                var responseModel = new PackageResponseModel(package);
                _unitOfWork.CommitTransaction();

                return BaseResponseModel<PackageResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception)
            {
                _unitOfWork.RollBack();
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task<BaseResponseModel<bool>> DeletePackage(Guid packageId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var package = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
                if (package == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Package not found");
                }

                package.IsDelete = true;
                package.DeletedTime = DateTime.UtcNow;
                package.DeletedBy = "Admin";
                package.IsActive = false;

                await _unitOfWork.PackageRepository.UpdateAsync(package);
                await _unitOfWork.SaveAsync();

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<bool>.OkResponseModel(true);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception)
            {
                _unitOfWork.RollBack();
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task<BaseResponseModel<PackageResponseModel>> GetPackageById(Guid packageId)
        {
            try
            {
             

                var package = await _unitOfWork.PackageRepository.GetByIdAsync(packageId);
                if (package == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Package not found");
                }
                return BaseResponseModel<PackageResponseModel>.OkResponseModel(new PackageResponseModel(package));
            }
            catch (CustomException)
            {
                
                throw;
            }
            catch (Exception)
            {
               
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
         
        }

        public async Task<BaseResponseModel<IEnumerable<PackageResponseModel>>> GetAllPackages()
        {
            try
            {
                var package = await _unitOfWork.PackageRepository.GetAllPackagesAsync();
                var responseModels = package.Select(p => new PackageResponseModel(p));
                return BaseResponseModel<IEnumerable<PackageResponseModel>>.OkResponseModel(responseModels);
            }
            catch (CustomException)
            {
            
                throw;
            }
            catch (Exception)
            {
          
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
         
        }
    }
}