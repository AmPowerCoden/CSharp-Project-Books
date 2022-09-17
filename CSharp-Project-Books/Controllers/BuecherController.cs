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
            List<BuchDTO> aktuelleBuecher = new List<BuchDTO>();
            List<BuchDTO> archivierteBuecher = new List<BuchDTO>();

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

        public void Verschieben(string titel, string quelle, string ziel) // hier abänderungen treffen
        {
            string connectionString = GetConnectionString();
            var repository = new BuecherRepository(connectionString);
            repository.VerschiebeBuch(titel, quelle, ziel);
        }

        [HttpGet]
        public IActionResult VerschiebeNachAktuell(BuchDTO buch)
        {
            var buchtitel = buch.Titel;

            Verschieben(buchtitel, "archivierte_buecher", "aktuelle_buecher");

            BuecherListModel model = LeseDatenInModel(GetConnectionString());

            return View("Views/Buecher/Index.cshtml", model);
        }

        [HttpGet]
        public IActionResult VerschiebeNachArchiviert(BuchDTO buch)
        {
            Verschieben(buch.Titel, "aktuelle_buecher", "archivierte_buecher");

            BuecherListModel model = LeseDatenInModel(GetConnectionString());

            return View("Views/Buecher/Index.cshtml", model);
        }

    }
}
