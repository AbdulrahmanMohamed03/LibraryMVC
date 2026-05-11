using Project.Core.RepositoriesAbstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }

        IAuthorRepository Authors { get; }
        IBookRepository Books { get; }
        ISubscriptionPlanRepository SubscriptionPlans { get; }
<<<<<<< usersubscription
        IUserSubscriptionRepository UserSubscriptions { get; }
=======
        IReservationRepository Reservations { get; }


        IAccountRepository Account { get; }
>>>>>>> main
        int Save();
    }
}
