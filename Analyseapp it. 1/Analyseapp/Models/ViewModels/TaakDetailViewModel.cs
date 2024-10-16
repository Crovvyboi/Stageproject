namespace Analyseapp.Models.ViewModels
{
    public class TaakDetailViewModel
    {
        public TaakModel taak { get; private set; }
        public List<UitvoertijdModel> uitvoertijden { get; private set; }
        public int? maintaakID { get; private set; }

        public TimeSpan gemUitvoertijd { get; private set; }

        public int keerUitgevoerd { get; private set; }
        public int trendKeerUitgevoerd { get; private set; }
        public TimeSpan trendGemUitvoertijd { get; private set; }

        public TaakDetailViewModel(
            TaakModel taak, 
            List<UitvoertijdModel> uitvoertijden, 
            int? maintaakID, 
            TimeSpan gemUitvoertijd,
            int keerUitgevoerd,
            int trendKeeruitgevoerd,
            TimeSpan trendGemUitvoerdtijd
            )
        {
            this.taak = taak;
            this.uitvoertijden = uitvoertijden;
            this.maintaakID = maintaakID;
            this.gemUitvoertijd = gemUitvoertijd;
            this.keerUitgevoerd = keerUitgevoerd;
            this.trendKeerUitgevoerd = trendKeeruitgevoerd;
            this.trendGemUitvoertijd = trendGemUitvoerdtijd;
        }

        public TaakDetailViewModel()
        {
                
        }
    }
}
