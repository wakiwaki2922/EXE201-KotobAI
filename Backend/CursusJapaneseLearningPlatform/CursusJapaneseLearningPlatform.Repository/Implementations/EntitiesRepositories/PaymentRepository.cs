using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using CursusJapaneseLearningPlatform.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Repository.Interfaces.PaymentManagementRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Implementations.EntitiesRepositories
{
    
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
        public async Task<IEnumerable<Payment>> GetAllPaymentHistoryAsync()
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Package)
                .Where(p => !p.IsDelete)
                .OrderByDescending(p => p.CreatedTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentHistoryByUserIdAsync(Guid userId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Package)
                .Where(p => p.UserId == userId && !p.IsDelete)
                .OrderByDescending(p => p.CreatedTime)
                .ToListAsync();
        }

        public async Task<Payment> GetBySubscriptionIdAsync(Guid subscriptionId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Package)
                .FirstOrDefaultAsync(p =>
                    p.SubscriptionId == subscriptionId &&
                    p.IsActive &&
                    !p.IsDelete);
        }

        public async Task<Payment> GetByPaypalPaymentIdAsync(string paypalPaymentId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Package)
                .FirstOrDefaultAsync(p =>
                    p.PaypalPaymentId == paypalPaymentId &&
                    p.IsActive &&
                    !p.IsDelete);
        }
    }
}
