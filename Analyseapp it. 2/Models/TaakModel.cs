using AnalyseApp_it._2.Data.DataModels;

namespace AnalyseApp_it._2.Models
{
    public class TaakModel
    {
        public string id { get; set; }
        public DateTimeOffset dateTime { get; set; }
        public int addedColumnsCount { get; set; }
        public int removedColumnsCount { get; set; }
        public int changesCount { get; set; }
        public List<SubtaakModel> subtaken { get; set; }

        public TaakModel(TaakDMO taakDMO)
        {
            this.id = taakDMO.id;
            this.dateTime = taakDMO.dateTime;
            this.addedColumnsCount = taakDMO.addedColumnsCount;
            this.removedColumnsCount = taakDMO.removedColumnsCount;
            this.changesCount = taakDMO.changesCount;
            this.subtaken = new List<SubtaakModel>();
        }
    }
}
