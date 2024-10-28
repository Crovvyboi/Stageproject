using AnalyseApp_it._2.Data.DataModels;
using System.ComponentModel;

namespace AnalyseApp_it._2.Models
{
    public class TaakModel
    {
        [DisplayName("Id")]
        public string id { get; private set; }
        [DisplayName("Datum - Tijd")]
        public DateTimeOffset dateTime { get; private set; }
        [DisplayName("Toegevoegde kolommen")]
        public int addedColumnsCount { get; private set; }
        [DisplayName("Verwijderde kolommen")]
        public int removedColumnsCount { get; private set; }
        [DisplayName("Gedetecteerde verschillen")]
        public int changesCount { get; private set; }
        public List<SubtaakModel> subtaken { get; private set; }

        public TaakModel(TaakDMO taakDMO)
        {
            this.id = taakDMO.id;
            this.dateTime = taakDMO.dateTime;
            this.addedColumnsCount = taakDMO.addedColumnsCount;
            this.removedColumnsCount = taakDMO.removedColumnsCount;
            this.changesCount = taakDMO.changesCount;
            this.subtaken = taakDMO.subtaken;
        }

        public TaakModel()
        {
            
        }

        public string GetDateTimeString()
        {
            return dateTime.ToString("dd MMM yyyy HH:mm:ss");
        }

        public List<string> GetOverzichten()
        {
            List<string> result = new List<string>();
            foreach (SubtaakModel subtaak in subtaken)
            {
                if (!result.Contains(subtaak.overzicht))
                {
                    result.Add(subtaak.overzicht);
                }
            }
            return result;
        }
    }
}
