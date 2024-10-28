using AnalyseApp_it._2.Data.DataModels;

namespace AnalyseApp_it._2.Models
{
    public class OverzichtModel
    {
        public string overzichtNaam { get; private set; }

        public List<OverzichtListitem> subtaken { get; private set; }


        public OverzichtModel(OverzichtDMO overzichtDMO)
        {
            this.overzichtNaam = overzichtDMO.overzichtNaam;
            this.subtaken = overzichtDMO.subtaken;
        }

        public OverzichtModel()
        {
            
        }
    }

    public struct OverzichtListitem
    {
        DateTimeOffset datetime;
        SubtaakModel subtaak;

        public OverzichtListitem(DateTimeOffset datetime, SubtaakModel subtaak)
        {
            this.datetime = datetime;
            this.subtaak = subtaak;
        }
    }
}
