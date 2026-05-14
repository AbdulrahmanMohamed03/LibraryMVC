using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Enums
{
    public enum TransactionType : byte
    {
        BorrowFee = 1,
        Fine = 2,
        Subscription = 3,
        Damaged = 4
    }
}
