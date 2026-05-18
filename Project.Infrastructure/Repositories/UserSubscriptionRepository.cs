using Microsoft.EntityFrameworkCore;
using Project.Core.Models;
using Project.Core.RepositoriesAbstraction;
using Project.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.Repositories
{
    public class UserSubscriptionRepository : BaseRepository<UserSubscription>, IUserSubscriptionRepository
    {
        public UserSubscriptionRepository(AppDbContext _context) : base(_context)
        {
        }

        public async Task<UserSubscription?> GetActiveSubscriptionByUserAsync(string userId)
        {
            return await _context.UserSubscriptions
                .Include(u => u.Plan)
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.UserId == userId
                                       && u.IsActive
                                       && u.EndDate > DateTime.Now);
        }

        public async Task<IEnumerable<UserSubscription>> GetAllWithDetailsAsync()
        {
            return await _context.UserSubscriptions
                .Include(u => u.Plan)
                .Include(u => u.User)
                .OrderByDescending(u => u.StartDate)
                .ToListAsync();
        }

        public async Task<UserSubscription?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.UserSubscriptions
                .Include(u => u.Plan)
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<UserSubscription>> GetSubscriptionHistoryAsync(string userId)
        {
            return await _context.UserSubscriptions
                .Include(u => u.Plan)
                .Include(u => u.User)
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.StartDate)
                .ToListAsync();
        }
        public async Task<UserSubscription?> GetExpiredSubscriptionByUserAsync(string userId)
        {
            return await _context.UserSubscriptions
                .Include(u => u.Plan)
                .Where(u => u.UserId == userId
                         && u.IsActive
                         && u.EndDate <= DateTime.Now  
                         && u.Plan.Name != "Free")     
                .OrderByDescending(u => u.EndDate)
                .FirstOrDefaultAsync();
        }
    }
}
