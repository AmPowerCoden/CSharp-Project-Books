using CSharp_Project_Books.Utils;
using Microsoft.AspNetCore.Mvc;
using BuecherDB;
using CSharp_Project_Books.Models;
using System.Xml.Serialization;

namespace CSharp_Project_Books.Controllers
{
    public class BuecherController : Controller
    {
        private readonly KonfigurationsLeser _konfigurationsLeser;

        public BuecherController(KonfigurationsLeser konfigurationsLeser)
        {
            this._konfigurationsLeser = konfigurationsLeser;
        }

        public string GetConnectionString()
        {
            return _konfigurationsLeser.LiesDatenbankVerbindungZurMariaDB();
        }

        public IActionResult Index()
        {

            BuecherListModel model = LeseDatenInModel(GetConnectionString());

            return View(model);
        }

        public BuecherListModel LeseDatenInModel(string connectionString)
        {
            List<BuchDTO> aktuelleBuecher = new();
            List<BuchDTO> archivierteBuecher = new();

            var repository = new BuecherRepository(connectionString);

            Thread aktuelleBuecherLesen = new Thread(() =>
            {
                aktuelleBuecher = repository.HoleAktuelleBuecher();
            });

            Thread archivierteBuecherLesen = new Thread(() =>
            {
                archivierteBuecher = repository.HoleArchivierteBuecher();
            });

            aktuelleBuecherLesen.Start();
            archivierteBuecherLesen.Start();

            aktuelleBuecherLesen.Join();
            archivierteBuecherLesen.Join();

            return new BuecherListModel(aktuelleBuecher, archivierteBuecher);
        }

        public void Verschieben(BuchDTO buch, string quelle, string ziel) // hier abänderungen treffen
        {
            string connectionString = GetConnectionString();
            var repository = new BuecherRepository(connectionString);
            repository.VerschiebeBuch(buch, quelle, ziel);
        }

        public IActionResult VerschiebeNachAktuell(BuchDTO buch)
        {
            Verschieben(buch, "archivierte_buecher", "aktuelle_buecher");

            BuecherListModel model = LeseDatenInModel(GetConnectionString());

            return View("Views/Buecher/Index.cshtml", model);
        }

        public IActionResult VerschiebeNachArchiviert(BuchDTO buch)
        {
            Verschieben(buch, "aktuelle_buecher", "archivierte_buecher");

            BuecherListModel model = LeseDatenInModel(GetConnectionString());

            return View("Views/Buecher/Index.cshtml", model);
        }

    }
}
