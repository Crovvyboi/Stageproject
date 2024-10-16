using System.ComponentModel.DataAnnotations;

namespace Analyseapp.Data.Datamodels
{
    public class UitvoertijdDMO
    {
        public int uitvoertijdID { get; private set; }
        public DateOnly startDatum { get; private set; }
        public TimeOnly startTijd { get; private set; }
        public DateOnly eindDatum { get; private set; }
        public TimeOnly eindTijd { get; private set; }
        public TimeSpan totUitvoertijd { get; private set; }

        public UitvoertijdDMO(int uitvoertijdID, DateOnly startDatum, TimeOnly starttijd, DateOnly eindDatum, TimeOnly eindTijd, TimeSpan totUitvoertijd)
        {
            this.uitvoertijdID = uitvoertijdID;
            this.startDatum = startDatum;
            this.startTijd = starttijd;
            this.eindDatum = eindDatum;
            this.eindTijd = eindTijd;
            this.totUitvoertijd = totUitvoertijd;
        }
    }
}
