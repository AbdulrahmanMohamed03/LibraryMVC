using Microsoft.AspNetCore.Mvc;
using Project.Application.Services.Interfaces;

namespace Project.MVC.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService transaction;
        public TransactionController(ITransactionService _transaction)
        {
            transaction = _transaction;
        }
        public IActionResult Index()
        {
            transaction.getall();
            return View(transaction.getall());
        }
    }
}
