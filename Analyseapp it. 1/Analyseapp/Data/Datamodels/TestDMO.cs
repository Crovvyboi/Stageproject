using Analyseapp.Models;
using Analyseapp.Util;

namespace Analyseapp.Data.Datamodels
{
    public class TestDMO
    {
        public int testId { get; private set; }
        public string testNaam { get; private set; }
        public string testBeschrijving { get; private set; }
        public ProgrammaModel programma {  get; private set; }
        public List<TestModel> subtest { get; private set; }
        public SlagingsUitkomst slagingsUitkomst { get; private set; }
        public UitvoertijdModel uitvoertijd { get; private set; }

        public TestDMO(
            int testId, 
            string testNaam, 
            string testBeschrijving,
            ProgrammaModel programma,
            List<TestModel> subtest, 
            SlagingsUitkomst slagingsUitkomst, 
            UitvoertijdModel uitvoertijd
            )
        {
            this.testId = testId;
            this.testNaam = testNaam;
            this.testBeschrijving = testBeschrijving;
            this.programma = programma;
            this.subtest = subtest;
            this.slagingsUitkomst = slagingsUitkomst;
            this.uitvoertijd = uitvoertijd;
        }

        public TestDMO()
        {
            
        }

        public void AddSubtest(TestModel test)
        {
            subtest.Add(test);
        }
    }
}
