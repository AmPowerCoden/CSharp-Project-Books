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
            return GetBooksFromOneTable("aktuelle_Buecher");
        }

        public List<BuchDTO> HoleArchivierteBuecher()
        {
            return GetBooksFromOneTable("archivierte_Buecher");
        }

        public List<BuchDTO> GetBooksFromOneTable(string tabellenname)
        {
            List<BuchDTO> Buecher = new();

            using var db_Verbindung = new MySqlConnection(connectionString);
            db_Verbindung.Open();

            string queryBuecher = "SELECT titel, autor FROM " + tabellenname;
            using var commando = new MySqlCommand(queryBuecher, db_Verbindung);
            using var reader = commando.ExecuteReader();

            while (reader.Read())
            {
                BuchDTO buch = new BuchDTO
                {
                    isAktuell = true,
                    Autor = (string?)reader["autor"],
                    Titel = (string?)reader["titel"]
                };

                Buecher.Add(buch);
            }

            db_Verbindung.Close();

            return Buecher;
        }

        public void VerschiebeBuch(BuchDTO buch, string quelle, string ziel)
        {
            using var db_Verbindung = new MySqlConnection(connectionString);
            db_Verbindung.Open();

            LoescheBuch(buch, quelle);

            FuegeBuchEin(buch, ziel);

            db_Verbindung.Close();
        }

        public void LoescheBuch(BuchDTO buch, string quelle)
        {
            using var db_Verbindung = new MySqlConnection(connectionString);
            db_Verbindung.Open();

            string queryBuchLoesche = "DELETE FROM " + quelle + " WHERE titel = @titel AND autor = @autor";
            using var command = new MySqlCommand(queryBuchLoesche, db_Verbindung);
            command.Parameters.AddWithValue("titel", buch.Titel);
            command.Parameters.AddWithValue("autor", buch.Autor);

            command.ExecuteNonQuery();

            db_Verbindung.Close();
        }

        public void FuegeBuchEin(BuchDTO buch, string ziel)
        {
            using var db_Verbindung = new MySqlConnection(connectionString);
            db_Verbindung.Open();

            string queryBuchLoesche = "INSERT INTO " + ziel + " (titel, autor) VALUES ('" + buch.Titel + "', '" + buch.Autor + "')";
            using var command = new MySqlCommand(queryBuchLoesche, db_Verbindung);
            command.Parameters.AddWithValue("titel", buch.Titel);
            command.Parameters.AddWithValue("autor", buch.Autor);

            command.ExecuteNonQuery();

            db_Verbindung.Close();
        }
    }
}