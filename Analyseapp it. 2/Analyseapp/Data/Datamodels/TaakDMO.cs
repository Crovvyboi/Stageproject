using Analyseapp.Models;

namespace Analyseapp.Data.Datamodels
{
    public class TaakDMO
    {
        public int taakID { get; private set; }
        public string taakName { get; private set; }
        public ProgrammaModel programma {  get; private set; }
        public string taakMessage {  get; private set; }

        public List<TaakModel> subtaken {  get; private set; }  
        public UitvoertijdModel uitvoertijd { get; private set; }

        public TaakDMO(int taakId, string taakNaam, ProgrammaModel programma, string taakMessage, List<TaakModel> subtaken, UitvoertijdModel uitvoertijd)
        {
            this.taakID = taakId;
            this.taakName = taakNaam;
            this.programma = programma;
            this.taakMessage = taakMessage;
            this.subtaken = subtaken;
            this.uitvoertijd = uitvoertijd;
        }

        public TaakDMO()
        {
                
        }
    }

}
