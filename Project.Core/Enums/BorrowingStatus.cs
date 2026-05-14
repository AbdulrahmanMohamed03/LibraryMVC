using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Enums
{
    public enum BorrowingStatus : byte
    {
        Pending = 0,
        Active = 1,
        ReturnedOverdue = 2,
        Returned = 3,
        ReturnedDamaged = 4,
    }
}
