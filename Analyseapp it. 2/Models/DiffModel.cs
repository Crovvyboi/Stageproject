namespace AnalyseApp_it._2.Models
{
    public class DiffModel
    {
        public string onEntryId { get; private set; }
        public string onColumn { get; private set; }
        public string valueA { get; private set; }
        public string valueB { get; private set; }

        public DiffModel(string onEntryId, string onColumn, string valueA, string valueB)
        {
            this.onEntryId = onEntryId;
            this.onColumn = onColumn;
            this.valueA = valueA;
            this.valueB = valueB;
        }
    }
}
