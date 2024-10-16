namespace Analyseapp.Models.ViewModels
{
    public class RapportOverviewViewModel
    {
        public List<RapportModel> rapporten { get; private set; }
        public List<ProgrammaModel> programmas { get; private set; }

        public RapportOverviewViewModel(List<RapportModel> rapporten, List<ProgrammaModel> programmas)
        {
            this.rapporten = rapporten;
            this.programmas = programmas;
        }

        public bool CheckIfRapportContains(ProgrammaModel prog)
        {
            DateOnly nowDate = DateOnly.FromDateTime(DateTime.Now);
            if (rapporten.Where(x => x.programma.programmaID.Equals(prog.programmaID)).Where(x => x.madeOn.CompareTo(nowDate) == 0 ).Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
