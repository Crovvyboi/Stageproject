namespace Analyseapp.Data.Datamodels
{
    public class ProgrammaDMO
    {
        public int programmaID { get; private set; }
        public string programmaName { get; private set; }
        public string programmaVersie {  get; private set; }
        public bool isInDevelopment { get; private set; }

        public ProgrammaDMO(int programmaID, string programmaName, string programmaVersie, bool isInDevelopment)
        {
            this.programmaID = programmaID;
            this.programmaName = programmaName;
            this.programmaVersie = programmaVersie;
            this.isInDevelopment = isInDevelopment;
        }

        public ProgrammaDMO()
        {
            
        }
    }
}
