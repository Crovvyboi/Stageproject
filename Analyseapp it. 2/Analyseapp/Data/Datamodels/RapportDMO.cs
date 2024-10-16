using Analyseapp.Models;

namespace Analyseapp.Data.Datamodels
{
    public class RapportDMO
    {
        public int rapportId { get; private set; }
        public int testKerenUitgevoerd { get; private set; }
        public int testSlagingskans { get; private set; }
        public int taakKerenUitgevoerd { get; private set; }
        public TrendModel trend { get; private set; }
        public ProgrammaModel programma { get; private set; }
        public DateOnly madeOn { get; private set; }

        public TimeSpan testGemUitvoertijd { get; private set; }
        public TimeSpan taakGemUitvoertijd { get; private set; }

        public RapportDMO(
            int rapportId, 
            int testKerenUitgevoerd, 
            int testSlagingskans, 
            int taakKerenUitgevoerd, 
            TrendModel trend, 
            ProgrammaModel programma,
            DateOnly madeOn,
            TimeSpan testGemUitvoertijd,
            TimeSpan taakGemUitvoertijd
            )
        {
            this.rapportId = rapportId;
            this.testKerenUitgevoerd = testKerenUitgevoerd;
            this.testSlagingskans = testSlagingskans;
            this.taakKerenUitgevoerd = taakKerenUitgevoerd;
            this.trend = trend;
            this.programma = programma;
            this.madeOn = madeOn;
            this.testGemUitvoertijd = testGemUitvoertijd;
            this.taakGemUitvoertijd = taakGemUitvoertijd;
        }

        public RapportDMO()
        {
            
        }
    }
}
