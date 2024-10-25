namespace AnalyseApp_it._2.Models
{
    public class DiffModel
    {
        public string onEntryId { get; set; }
        public string onColumn { get; set; }
        public string valueA { get; set; }
        public string valueB { get; set; }

        public DiffModel(string onEntryId, string onColumn, string valueA, string valueB)
        {
            this.onEntryId = onEntryId;
            this.onColumn = onColumn;
            this.valueA = valueA;
            this.valueB = valueB;
        }
    }
}
