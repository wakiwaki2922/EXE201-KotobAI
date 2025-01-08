using CursusJapaneseLearningPlatform.Repository.Bases.BaseEntitys;
using CursusJapaneseLearningPlatform.Repository.Implementations.CollectionManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Implementations.EntitiesRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.CollectionManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.EntityRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.FlashcardRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.PackageManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.PaymentManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.SubcriptionManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.UserManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.VocabularyManagementRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IFlashcardRepository FlashcardRepository { get; }
    ISubscriptionRepository SubscriptionRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    IPackageRepository PackageRepository { get; }
    IVocabularyRepository VocabularyRepository { get; }
    ICollectionRepository CollectionRepository { get; }
    IUserRepository UserRepository { get; }
    IChatRepository ChatRepository { get; }
    IMessageRepository MessageRepository { get; }
    IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity;
    void Save();
    Task SaveAsync();
    void BeginTransaction();
    void CommitTransaction();
    void RollBack();
}
