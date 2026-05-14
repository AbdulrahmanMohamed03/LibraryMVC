using Project.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.BorrowingRecord
{
    public class CreateBorrowVM
    {
        public int? BorrowingRecordId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
