namespace CSharp_Project_Books.Utils
{
    //Erstellen des Konfigurationslesers, welcher später die Konfiguration auslesen kann und somit den connectionString zur Datenbank ausliest
    public class KonfigurationsLeser : IKonfigurationsLeser
    {
        private readonly IConfiguration _configuration;

        public KonfigurationsLeser(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string LiesDatenbankVerbindungZurMariaDB()
        {
            return _configuration.GetConnectionString("MariaDB");
        }
    }

    public interface IKonfigurationsLeser
    {
        string LiesDatenbankVerbindungZurMariaDB();
    }
}
