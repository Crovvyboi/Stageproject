using Analyseapp.Data.Datamodels;
using Analyseapp.Util;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Analyseapp.Models
{
    public class TestModel
    {
        [DisplayName("Id")]
        public int Id { get; private set; }

        [DisplayName("Naam")]
        public string testNaam { get; private set; }

        [DisplayName("Beschrijving")]
        public string testBeschrijving { get; private set; }

        public ProgrammaModel programma { get; private set; }


        public List<TestModel> subtest { get; private set; }

        [DisplayName("Uitkomst")]
        public SlagingsUitkomst slagingsUitkomst { get; private set; }
        public UitvoertijdModel uitvoertijd { get; private set; }

        public TestModel(TestDMO testDMO)
        {
            this.Id = testDMO.testId;
            this.testNaam = testDMO.testNaam;
            this.testBeschrijving = testDMO.testBeschrijving;
            this.subtest = testDMO.subtest;
            this.slagingsUitkomst = testDMO.slagingsUitkomst;
            this.uitvoertijd = testDMO.uitvoertijd;

            programma = testDMO.programma;

        }

        public TestModel()
        {
            
        }
    }
}
