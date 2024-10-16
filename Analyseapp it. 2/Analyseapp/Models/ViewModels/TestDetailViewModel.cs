using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Analyseapp.Models.ViewModels
{
    public class TestDetailViewModel
    {
        public TestModel testmodel { get; private set; }
        public List<UitvoertijdModel> uitvoertijdenAlleTesten { get; private set; }

        public int? mainTest { get; private set; }
        // public int? rapportHeadtest { get; private set; }
        // public bool hasRapport {  get; private set; }
        public double slagingspercentage { get; private set; }
        public int kerenUitgevoerd { get; private set; }
        public TimeSpan gemUitvoertijd { get; private set; }
        public TrendModel trendModel { get; private set; }

        public TestDetailViewModel(
            TestModel testModel, 
            List<UitvoertijdModel> uitvoertijdenAlleTesten, 
            int? mainTest, 
            double slagingspercentage,
            int kerenuitgevoerd,
            TimeSpan gemUitvoertijd,
            TrendModel trendModel
            )
        {
            this.testmodel = testModel;
            this.uitvoertijdenAlleTesten = uitvoertijdenAlleTesten;
            this.mainTest = mainTest;
            this.kerenUitgevoerd = kerenuitgevoerd;
            this.gemUitvoertijd = gemUitvoertijd;
            this.slagingspercentage = slagingspercentage;
            this.trendModel = trendModel;
        }

        public TestDetailViewModel()
        {
            
        }



    }
}
