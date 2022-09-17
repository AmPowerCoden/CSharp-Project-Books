using CSharp_Project_Books.Utils;
using Microsoft.AspNetCore.Mvc;
using BuecherDB;

namespace CSharp_Project_Books.Controllers
{
    public class BuecherController : Controller
    {
        private readonly KonfigurationsLeser _konfigurationsLeser;

        public BuecherController(KonfigurationsLeser konfigurationsLeser)
        {
            this._konfigurationsLeser = konfigurationsLeser;
        }

        public IActionResult Index()
        {
            string connectionString = GetConnectionString();
            var repository = new BuecherRepository(connectionString);

            return View();
        }

        public string GetConnectionString()
        {
            return _konfigurationsLeser.LiesDatenbankVerbindungZurMariaDB();
        }
    }
}
