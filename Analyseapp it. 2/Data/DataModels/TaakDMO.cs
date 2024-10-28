using AnalyseApp_it._2.Models;

namespace AnalyseApp_it._2.Data.DataModels
{
    public class TaakDMO
    {
        public string id { get; set; }
        public DateTimeOffset dateTime { get; set; }
        public int addedColumnsCount { get; set; }
        public int removedColumnsCount { get; set; }
        public int changesCount { get; set; }
        public List<SubtaakModel> subtaken { get; set; }

        public TaakDMO(string id, DateTimeOffset dateTime, int added, int removed, int changes, List<SubtaakModel> subtaken)
        {
            this.id = id;
            this.dateTime = dateTime;
            this.addedColumnsCount = added;
            this.removedColumnsCount = removed;
            this.changesCount = changes;
            this.subtaken = subtaken;
        }

        public TaakDMO()
        {
            
        }
    }
}
