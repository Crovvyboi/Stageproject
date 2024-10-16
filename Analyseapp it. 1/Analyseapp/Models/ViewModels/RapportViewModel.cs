namespace Analyseapp.Models.ViewModels
{
    public class RapportViewModel
    {
        public RapportModel rapport { get; private set; }
        public List<TestModel> testModels {  get; private set; }
        public List<TaakModel> taakModels { get; private set; }

        public RapportViewModel(RapportModel rapport, List<TestModel> testModels, List<TaakModel> taakModels)
        {
            this.rapport = rapport;
            this.testModels = testModels;
            this.taakModels = taakModels;

            this.testModels = this.testModels.OrderByDescending(x => x.uitvoertijd.startDatum).ThenBy(x => x.uitvoertijd.startTijd).ToList();
            this.taakModels = this.taakModels.OrderByDescending(x => x.uitvoertijd.startDatum).ThenBy(x => x.uitvoertijd.startTijd).ToList();
        }

        public RapportViewModel()
        {
            
        }

        public double GetTestUitvoertijdOnIndex(int index)
        {
            if (testModels.Count > index)
            {
                TestModel testModel = testModels[index];
                return testModel.uitvoertijd.totUitvoertijd.TotalMinutes ;
            }
            return 0;
        }

        public DateOnly GetTestDateOnIndex(int index)
        {
            if (testModels.Count > index)
            {
                TestModel testmodel = testModels[index];
                return testmodel.uitvoertijd.startDatum;
            }
            return testModels.Last().uitvoertijd.startDatum.AddDays(-1);
        }

        public double GetTaakUitvoertijdOnIndex(int index)
        {
            if (taakModels.Count > index)
            {
                TaakModel taakmodel = taakModels[index];
                return taakmodel.uitvoertijd.totUitvoertijd.TotalMinutes;
            }
            return 0;
        }

        public DateOnly GetTaakDateOnIndex(int index)
        {
            if (taakModels.Count > index)
            {
                TaakModel taakmodel = taakModels[index];
                return taakmodel.uitvoertijd.startDatum;
            }
            return taakModels.Last().uitvoertijd.startDatum.AddDays(-1);
        }
    }
}
