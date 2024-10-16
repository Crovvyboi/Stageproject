using Analyseapp.Data.Datamodels;
using System.ComponentModel.DataAnnotations;

namespace Analyseapp.Models
{
    public class UitvoertijdModel
    {
        public int uitvoertijdID { get; private set; }
        public DateOnly startDatum { get; private set; }
        public TimeOnly startTijd { get; private set; }
        public DateOnly eindDatum { get; private set; }
        public TimeOnly eindTijd { get; private set; }
        public TimeSpan totUitvoertijd { get; private set; }

        public UitvoertijdModel(int uitvoertijdID, DateOnly startDatum, TimeOnly starttijd, DateOnly eindDatum, TimeOnly eindTijd, TimeSpan totUitvoertijd)
        {
            this.uitvoertijdID = uitvoertijdID;
            this.startDatum = startDatum;
            this.startTijd = starttijd;
            this.eindDatum = eindDatum;
            this.eindTijd = eindTijd;
            this.totUitvoertijd = totUitvoertijd;
        }

        public UitvoertijdModel(UitvoertijdDMO uitvoertijdDMO)
        {
            this.uitvoertijdID = uitvoertijdDMO.uitvoertijdID;
            this.startDatum = uitvoertijdDMO.startDatum;
            this.startTijd = uitvoertijdDMO.startTijd;
            this.eindDatum = uitvoertijdDMO.eindDatum;
            this.eindTijd = uitvoertijdDMO.eindTijd;
            this.totUitvoertijd = uitvoertijdDMO.totUitvoertijd;
        }

        public UitvoertijdModel()
        {
            
        }

        public void CalculateTotUitvoertijd()
        {
            DateTime startDateTime = new DateTime(startDatum, startTijd);
            DateTime eindDateTime = new DateTime(eindDatum, eindTijd);

            totUitvoertijd = eindDateTime - startDateTime;
        }

        public DateTime GetStartDateTime()
        {
            return new DateTime(startDatum, startTijd);
        }

        public DateTime GetEindDateTime()
        {
            return new DateTime(eindDatum, eindTijd);
        }
    }
}