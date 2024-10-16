namespace Analyseapp.Models
{
    public class TrendModel
    {
        public int trendId {  get; private set; }
        public int testVerschilKerenUitgevoerd { get; private set; }
        public double testVerschilSlagingskans { get; private set; }
        public TimeSpan testVerschilGemUitvoertijden { get; private set; }
        public int taakVerschilKerenUitgevoerd { get; private set; }
        public TimeSpan taakVerschilGemUitvoertijden { get; private set; }


        public TrendModel(int trendId, int verschiku, double verschilsk, TimeSpan verschilgut, int taakVerschilKerenUitgevoerd, TimeSpan taakVerschilGemUitvoertijden)
        {
            this.trendId = trendId;
            this.testVerschilKerenUitgevoerd = verschiku;
            this.testVerschilSlagingskans = verschilsk;
            this.testVerschilGemUitvoertijden = verschilgut;
            this.taakVerschilKerenUitgevoerd = taakVerschilKerenUitgevoerd;
            this.taakVerschilGemUitvoertijden = taakVerschilGemUitvoertijden;
        }

        public TrendModel()
        {
            
        }

        public void SetID(int id)
        {
            this.trendId = id;
        }

        // Bereken verschil in keren uitgevoerd
        public void CalculateVKU(int vku1, int vku2)
        {
            testVerschilKerenUitgevoerd = vku2 - vku1;
        }

        // Bereken verschil in slagingskans
        public void CalculateVSK(int slagingskans1, int slagingskans2)
        {
            testVerschilSlagingskans = slagingskans2 - slagingskans1;
        }

        // Bereken verschil in gemiddelde uitvoertijden
        public void CalculateVGU(TimeOnly vgu1, TimeOnly vgu2)
        {
            testVerschilGemUitvoertijden = vgu2 - vgu1;
        }
    }
}