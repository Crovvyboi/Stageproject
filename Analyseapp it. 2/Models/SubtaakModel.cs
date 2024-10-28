using System.ComponentModel;

namespace AnalyseApp_it._2.Models
{
    public class SubtaakModel
    {
        [DisplayName("Overzicht")]
        public string overzicht { get; private set; }
        [DisplayName("Added columns")]
        public string addedColumns { get; private set; }
        [DisplayName("Removed columns")]
        public string removedColumns { get; private set; }
        public List<DiffModel> foundDiffs { get; private set; }

        public SubtaakModel(string overzicht, string addedColumns, string removedColumns, List<DiffModel> foundDiffs)
        {
            this.overzicht = overzicht;
            this.addedColumns = addedColumns;
            this.removedColumns = removedColumns;
            this.foundDiffs = foundDiffs;
        }
    }
}
