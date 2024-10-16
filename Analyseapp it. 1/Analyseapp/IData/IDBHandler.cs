using Analyseapp.Data.Datamodels;
using Analyseapp.Models;

namespace Analyseapp.IData
{
    public interface IDBHandler
    {
        public List<TestDMO> GetAllTests();
        public TestDMO GetTestByID(int testid);
        public List<UitvoertijdDMO> GetAllUitvoertijdenOnTest(string testnaam);
        public int GetTestIDOnDate(DateOnly date, TimeOnly time, string testname);
        public List<TestDMO> GetTestsAtProgramma(int progId);


        public List<TaakDMO> GetAllTaken();
        public TaakDMO GetTaakAtID(int taakID);
        public List<UitvoertijdDMO> GetAllUitvoertijdenOnTaak(string taaknaam);
        public int GetTaakIDOnDate(DateOnly date, TimeOnly time, string taaknaam);
        public int? GetMaintaakOnSubtaak(int taakID);
        public List<TaakDMO> GetTakenAtProgramma(int progId);



        public List<RapportDMO> GetRapporten();
        public RapportDMO GetRapport(int rapportID);
        public List<RapportDMO> GetRapportenOfProgramma(int progID);
        public int? GetMaintestOnSubtest(int testid);
        public bool GetRapportOnTest(int testid);
        public List<ProgrammaDMO> GetProgrammas();
        public ProgrammaDMO GetProgramma(int programmaID);

        public int InsertTrend(TrendModel trend);
    }
}
