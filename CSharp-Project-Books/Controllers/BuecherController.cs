using CSharp_Project_Books.Utils;
using Microsoft.AspNetCore.Mvc;
using BuecherDB;
using CSharp_Project_Books.Models;

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

            List<BuchDTO>? aktuelleBuecher = repository.HoleAktuelleBuecher();
            List<BuchDTO>? archivierteBuecher = repository.HoleArchivierteBuecher();

            var model = new BuecherListModel(aktuelleBuecher, archivierteBuecher);

            return View(model);
        }

        public string GetConnectionString()
        {
            return _konfigurationsLeser.LiesDatenbankVerbindungZurMariaDB();
        }
    }
}
