using CSharp_Project_Books.Utils;
using Microsoft.AspNetCore.Mvc;
using BuecherDB;
using CSharp_Project_Books.Models;
using System.Xml.Serialization;
using System;

namespace CSharp_Project_Books.Controllers
{
    public class BuecherController : Controller
    {
        private readonly KonfigurationsLeser _konfigurationsLeser;

        public BuecherController(KonfigurationsLeser konfigurationsLeser)
        {
            this._konfigurationsLeser = konfigurationsLeser; //_konfigurationsLeser wird gesetzt um connectionString auszulesen
        }

        public string GetConnectionString()
        {
            return _konfigurationsLeser.LiesDatenbankVerbindungZurMariaDB(); //auslesen des connectionStrings
        }

        public IActionResult Index()
        {

            BuecherListModel model = LeseDatenInModel(GetConnectionString()); //Daten werden in ein Model eingelesen

            return View(model); //View wird erstellt
        }

        public BuecherListModel LeseDatenInModel(string connectionString)
        {
            //erstellen zweier Listen, welche die Daten der angezeigten Tabellen beinhaltet
            List<BuchDTO> aktuelleBuecher = new();
            List<BuchDTO> archivierteBuecher = new();

            //Initialisieren des repositorys, in welchem die Datenbankabfragen stattfinden
            var repository = new BuecherRepository(connectionString);

            //Auslesen der beiden Tabellen und befüllen der Listen mit den Daten in zwei Unterschiedlichen Threads (zur Parallelisierung)
            Thread aktuelleBuecherLesen = new Thread(() =>
            {
                aktuelleBuecher = repository.HoleAktuelleBuecher();
            });

            Thread archivierteBuecherLesen = new Thread(() =>
            {
                archivierteBuecher = repository.HoleArchivierteBuecher();
            });

            //Ausführen der oben erstellten Threads
            aktuelleBuecherLesen.Start();
            archivierteBuecherLesen.Start();

            aktuelleBuecherLesen.Join();
            archivierteBuecherLesen.Join();

            return new BuecherListModel(aktuelleBuecher, archivierteBuecher);
        }

        public void Verschieben(BuchDTO buch, string quelle, string ziel)
        {
            string connectionString = GetConnectionString();
            //Repository zum verschieben wird Initialisiert
            var repository = new BuecherRepository(connectionString);
            repository.VerschiebeBuch(buch, quelle, ziel); //Verschieben des Buches (Löschen aus der 1. Tabelle und Einfügen in die andere Tabelle) wird ausgeführt
        }

        public IActionResult VerschiebeNachAktuell(BuchDTO buch)
        {
            Verschieben(buch, "archivierte_buecher", "aktuelle_buecher"); //Start des verschiebens eines archivierten Buches, zu einem aktuellen Buch

            BuecherListModel model = LeseDatenInModel(GetConnectionString()); //Neues Laden der beiden Tabellen, nach obiger Verschiebung

            return View("Views/Buecher/Index.cshtml", model); //erstellen der View
        }

        public IActionResult VerschiebeNachArchiviert(BuchDTO buch)
        {
            Verschieben(buch, "aktuelle_buecher", "archivierte_buecher"); //Start des verschiebens eines aktuellen Buches, zu einem archivierten Buch

            BuecherListModel model = LeseDatenInModel(GetConnectionString()); //Neues Laden der beiden Tabellen, nach obiger Verschiebung

            return View("Views/Buecher/Index.cshtml", model); //erstellend der View
        }

    }
}
