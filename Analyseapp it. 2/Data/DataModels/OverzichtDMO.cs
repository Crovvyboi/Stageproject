using AnalyseApp_it._2.Models;

namespace AnalyseApp_it._2.Data.DataModels
{
    public class OverzichtDMO
    {
        public string overzichtNaam { get; private set; }
        public List<OverzichtListitem> subtaken { get; private set; }

        public OverzichtDMO(string overzichtNaam, List<OverzichtListitem> subtaken)
        {
            this.overzichtNaam = overzichtNaam;
            this.subtaken = subtaken;
        }
    }


}
