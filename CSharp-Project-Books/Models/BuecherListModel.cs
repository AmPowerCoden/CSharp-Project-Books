using BuecherDB;

namespace CSharp_Project_Books.Models
{
    public class BuecherListModel
    {

        public List<BuchDTO> AktuelleBuecherList { get; set; } = new();

        public List<BuchDTO> ArchivierteBuecherList { get; set; } = new();

        public BuecherListModel(IEnumerable<BuchDTO> aktuelleBuecher, IEnumerable<BuchDTO> archivierteBuecher)
        {
            foreach(BuchDTO buchDTO in aktuelleBuecher)
            {
                AktuelleBuecherList.Add(buchDTO);
            }

            foreach (BuchDTO buchDTO in archivierteBuecher)
            {
                ArchivierteBuecherList.Add(buchDTO);
            }
        }
    }
}
