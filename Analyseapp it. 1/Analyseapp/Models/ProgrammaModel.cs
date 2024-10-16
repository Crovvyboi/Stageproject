using Analyseapp.Data.Datamodels;
using System.ComponentModel;

namespace Analyseapp.Models
{
    public class ProgrammaModel
    {
        [DisplayName("ID")]
        public int programmaID {  get; private set; }

        [DisplayName("Programma")]
        public string programma { get; private set; }

        [DisplayName("Versie")]
        public string progVersie { get; private set; }

        [DisplayName("In development")]
        public bool isInDevelopment { get; private set; }

        public ProgrammaModel(int programmaID, string programma, string progVersie, bool isInDevelopment)
        {
            this.programmaID = programmaID;
            this.programma = programma;
            this.progVersie = progVersie;
            this.isInDevelopment = isInDevelopment; 
        }

        public ProgrammaModel(ProgrammaDMO programmaDMO)
        {
            this.programmaID = programmaDMO.programmaID;
            this.programma = programmaDMO.programmaName;
            this.progVersie = programmaDMO.programmaVersie;
            this.isInDevelopment = programmaDMO.isInDevelopment;
        }
    }
}
