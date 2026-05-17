using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Application.ViewModels.Admin
{
    public class AdminDashboardVM
    {
        public int CategoriesCount { get; set; }
        public int BooksCount { get; set; }
        public int LibrariansCount { get; set; }
        public int ReservationsCount { get; set; }
        public decimal TotalIncome { get; set; }
    }
}
