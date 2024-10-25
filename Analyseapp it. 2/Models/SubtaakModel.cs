namespace AnalyseApp_it._2.Models
{
    public class SubtaakModel
    {
        public string overzicht { get; set; }
        public string addedColumns { get; set; }
        public string removedColumns { get; set; }
        public List<DiffModel> foundDiffs { get; set; }

        public SubtaakModel(string overzicht, string addedColumns, string removedColumns, List<DiffModel> foundDiffs)
        {
            this.overzicht = overzicht;
            this.addedColumns = addedColumns;
            this.removedColumns = removedColumns;
            this.foundDiffs = foundDiffs;
        }
    }
}
