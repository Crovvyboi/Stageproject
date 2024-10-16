using Analyseapp.Data.Datamodels;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.ComponentModel;

namespace Analyseapp.Models
{
    public class RapportModel
    {
        [DisplayName("Rapport nummer")]
        public int rapportId { get; private set; }
        public ProgrammaModel programma { get; private set; }


        [DisplayName("Keren test uitgevoerd")]
        public int testKerenUitgevoerd { get; private set; }

        [DisplayName("Test slagingskans")]
        public int testSlagingskans { get; private set; }
        public TimeSpan testGemUitvoertijd { get; private set; }


        [DisplayName("Keren taak uitgevoerd")]
        public int taakKerenUitgevoerd { get; private set; }
        public TimeSpan taakGemUitvoertijd { get; private set; }


        public TrendModel trend { get; private set; }

        [DisplayName("Made on")]
        public DateOnly madeOn {  get; private set; }

        public RapportModel(RapportDMO rapportDMO)
        {
            this.rapportId = rapportDMO.rapportId;
            this.testKerenUitgevoerd = rapportDMO.testKerenUitgevoerd;
            this.testSlagingskans = rapportDMO.testSlagingskans;
            this.taakKerenUitgevoerd = rapportDMO.taakKerenUitgevoerd;
            this.trend = rapportDMO.trend;
            this.programma = rapportDMO.programma;
            this.madeOn = rapportDMO.madeOn;
            this.testGemUitvoertijd = rapportDMO.testGemUitvoertijd;
            this.taakGemUitvoertijd = rapportDMO.taakGemUitvoertijd;
        }

        public RapportModel(
            int kerenuitgevoerd, 
            int slagingskans, 
            int taakKerenUitgevoerd, 
            TrendModel trend, 
            ProgrammaModel programma, 
            DateOnly madeOn,
            TimeSpan testGemUitvoertijd,
            TimeSpan taakGemUitvoertijd)
        {
            this.testKerenUitgevoerd = kerenuitgevoerd;
            this.testSlagingskans= slagingskans;
            this.taakKerenUitgevoerd = taakKerenUitgevoerd;
            this.trend = trend;
            this.programma = programma;
            this.madeOn= madeOn;
            this.testGemUitvoertijd = testGemUitvoertijd;
            this.taakGemUitvoertijd = taakGemUitvoertijd;
        }

        public RapportModel()
        {
            
        }


        // Maak pdf van rapport (using Itext7)
        public void FileRapport()
        {
            string pdfPath = "MyPDF.pdf";
            using (PdfWriter writer = new PdfWriter(pdfPath))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);
                    document.Add(new Paragraph("Hello, World!"));
                    document.Close();
                }
            }

            Console.WriteLine("PDF generated successfully using iText 7.");
        }
    }
}
