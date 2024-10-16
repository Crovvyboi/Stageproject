using Analyseapp.Data.Datamodels;
using Analyseapp.Util;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Analyseapp.Models
{
    public class TaakModel
    {
        [DisplayName("Id")]
        public int taakID {  get; private set; }

        [DisplayName("Naam")]
        public string taakName { get; private set; }

        [DisplayName("Programma")]
        public ProgrammaModel programma { get; private set; }



        [DisplayName("Bericht")]
        public string taakMessage { get; private set; }


        public List<TaakModel> subtaken {  get; private set; }

        [DisplayName("Nature")]
        public TaakResultNature taakMessageNature { get; private set; }

        public UitvoertijdModel uitvoertijd { get; private set; }

        public TaakModel(TaakDMO taakDMO)
        {
            this.taakID = taakDMO.taakID;
            this.taakName = taakDMO.taakName;
            this.programma = taakDMO.programma;
            this.taakMessage = taakDMO.taakMessage;
            subtaken = taakDMO.subtaken;
            this.uitvoertijd = taakDMO.uitvoertijd;

            taakMessageNature = DetermineResultNature();
        }

        public TaakResultNature DetermineResultNature()
        {
            List<string> patternsGood = new List<string>()
            {
                "Good",
                "Success",
                "Succesvol"
            };
            List<string> patternsBad = new List<string>()
            {
                "Bad",
                "Fail",
                "Failed",
                "Failure",
                "Error",
                "Gefaald",
                "Fout"
            };

            bool isgood = false;
            bool isbad = false; 

            foreach (string item in patternsGood)
            {
                Regex regex = new Regex("(?i).*"+item+".*");
                if (regex.IsMatch(taakMessage))
                {
                    isgood = true;
                    break;
                }
            }

            foreach (string item in patternsBad)
            {
                Regex regex = new Regex("(?i).*" + item + ".*");
                if (regex.IsMatch(taakMessage))
                {
                    isbad = true;
                    break;
                }
            }

            if (isgood && !isbad) 
            {
                return TaakResultNature.Good;
            }
            else if (!isgood && isbad)
            {
                return TaakResultNature.Bad;
            }
            else
            {
                return TaakResultNature.Undetermined;
            }
        }
    }
}
