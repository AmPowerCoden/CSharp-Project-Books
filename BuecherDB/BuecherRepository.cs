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

            db_Verbindung.Close();

            return Buecher;
        }

        public void VerschiebeBuch(string titel, string quelle, string ziel)
        {
            using var db_Verbindung = new MySqlConnection(connectionString);
            db_Verbindung.Open();

            BuchDTO buch = new BuchDTO();

            string queryBuechVerschiebe = "SELECT titel, autor FROM " + quelle + " WHERE titel = @titel";
            using var command = new MySqlCommand(queryBuechVerschiebe, db_Verbindung);
            command.Parameters.AddWithValue("titel", titel);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                if(ziel == "aktuelle_buecher")
                {
                    buch.isAktuell = true;
                }
                else
                {
                    buch.isAktuell = false;
                }
                buch.Autor = (string?)reader["autor"];
                buch.Titel = (string?)reader["titel"];
            }

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