using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Core.Enums
{
    public enum ReservationStatus :  byte
    {
        Pending = 1,
        Ready = 2,
        Fulfilled = 3,
        Cancelled = 4
    }
}
