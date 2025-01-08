using Microsoft.EntityFrameworkCore.Storage;
using CursusJapaneseLearningPlatform.Repository.Bases.BaseEntitys;
using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Implementations.UserManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.UserManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Implementations.CollectionManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Implementations.EntitiesRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.EntityRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.CollectionManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.SubcriptionManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.PaymentManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.PackageManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.VocabularyManagementRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.FlashcardRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private IDbContextTransaction _transaction;
    private bool _disposed = false;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Repositories
    private IUserRepository _userRepository;
    private ICollectionRepository _collectionRepository;
    private IVocabularyRepository _vocabularyRepository;
    private IPackageRepository _packageRepository;
    private IPaymentRepository _paymentRepository;
    private ISubscriptionRepository _subscriptionRepository;
    private IChatRepository _chatRepository;
    private IMessageRepository _messageRepository;
    private IFlashcardRepository _flashcardRepository;
    public IFlashcardRepository FlashcardRepository
    {
        get { return _flashcardRepository ??= new FlashcardRepository(_dbContext); }
    }
    public IUserRepository UserRepository
    {
        get { return _userRepository ??= new UserRepository(_dbContext); }
    }

    public ICollectionRepository CollectionRepository
    {
        get { return _collectionRepository ??= new CollectionRepository(_dbContext); }
    }

    public IVocabularyRepository VocabularyRepository
    {
        get { return _vocabularyRepository ??= new VocabularyRepository(_dbContext); }
    }

    public IPackageRepository PackageRepository
    {
        get
        {
            return _packageRepository ??= new PackageRepository(_dbContext);
        }
    }
    public IPaymentRepository PaymentRepository
    {
        get
        {
            return _paymentRepository ??= new PaymentRepository(_dbContext);
        }
    }

    public ISubscriptionRepository SubscriptionRepository
    {
        get
        {
            return _subscriptionRepository ??= new SubscriptionRepository(_dbContext);
        }
    }

    public IChatRepository ChatRepository
    {
        get
        {
            return _chatRepository ??= new ChatRepository(_dbContext);
        }
    }

    public IMessageRepository MessageRepository
    {
        get
        {
            return _messageRepository ??= new MessageRepository(_dbContext);
        }
    }
    #endregion Repositories

    public IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity
    {
        return new GenericRepository<T>(_dbContext);
    }
    public void Save()
    {
        _dbContext.SaveChanges();
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void BeginTransaction()
    {
        _transaction = _dbContext.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        try
        {
            _dbContext.SaveChanges();
            _transaction?.Commit();
        }
        catch
        {
            RollBack();
            throw;
        }
    }

    public void RollBack()
    {
        _transaction?.Rollback();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
                _transaction?.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
