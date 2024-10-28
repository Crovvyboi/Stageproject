using System.ComponentModel;

namespace AnalyseApp_it._2.Models
{
    public class DiffModel
    {
        [DisplayName("On entry ID")]
        public string onEntryId { get; private set; }
        [DisplayName("On Column")]
        public string onColumn { get; private set; }
        [DisplayName("Value A")]
        public string valueA { get; private set; }
        [DisplayName("Value B")]
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
