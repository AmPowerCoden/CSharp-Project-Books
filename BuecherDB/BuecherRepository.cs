using MySqlConnector;

namespace BuecherDB
{
    public class BuecherRepository
    {
        public string connectionString;

        public BuecherRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<BuchDTO> HoleAktuelleBuecher()
        {
            return getBooksFromOneTable("aktuelle_Buecher");
        }

        public List<BuchDTO> HoleArchivierteBuecher()
        {
            return getBooksFromOneTable("archivierte_Buecher");
        }

        public List<BuchDTO> getBooksFromOneTable(string tabellenname)
        {
            List<BuchDTO> Buecher = new List<BuchDTO>();

            using var db_Verbindung = new MySqlConnection(connectionString);
            db_Verbindung.Open();

            string queryBuecher = "SELECT titel, autor FROM " + tabellenname;
            using var commando = new MySqlCommand(queryBuecher, db_Verbindung);
            using var reader = commando.ExecuteReader();

            while (reader.Read())
            {
                BuchDTO buch = new BuchDTO();
                buch.isAktuell = true;
                buch.Autor = (string?)reader["autor"];
                buch.Titel = (string?)reader["titel"];

                Buecher.Add(buch);
            }

            return Buecher;
        }
    }
}