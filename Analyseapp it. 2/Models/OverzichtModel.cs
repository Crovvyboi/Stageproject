using AnalyseApp_it._2.Data.DataModels;
using System;

namespace AnalyseApp_it._2.Models
{
    public class OverzichtModel
    {
        public string overzichtNaam { get; private set; }

        public List<OverzichtListitem> subtaken { get; private set; }


        public OverzichtModel(OverzichtDMO overzichtDMO)
        {
            this.overzichtNaam = overzichtDMO.overzichtNaam;
            this.subtaken = overzichtDMO.subtaken;
        }

        public OverzichtModel()
        {
            
        }

        public int GetKerenUitgevoerd()
        {
            return subtaken.Count;
        }

        public int GetTotaalGevondenVerschillen()
        {
            int tot = 0;
            foreach (OverzichtListitem subtaak in subtaken)
            {
                tot += subtaak.subtaak.foundDiffs.Count;
            }
            return tot;
        }

        public int GetKerenKolomToegevoegd()
        {
            int tot = 0;
            foreach (OverzichtListitem subtaak in subtaken)
            {
                if (subtaak.subtaak.addedColumns != "n/a")
                {
                    tot++;
                }
            }
            return tot;
        }

        public int GetKerenKolomVerwijderd()
        {
            int tot = 0;
            foreach (OverzichtListitem subtaak in subtaken)
            {
                if (subtaak.subtaak.removedColumns != "n/a")
                {
                    tot++;
                }
            }
            return tot;
        }
    }

    public struct OverzichtListitem
    {
        public DateTimeOffset datetime { get; private set; }
        public SubtaakModel subtaak { get; private set; }

        public OverzichtListitem(DateTimeOffset datetime, SubtaakModel subtaak)
        {
            this.datetime = datetime;
            this.subtaak = subtaak;
        }

        public string GetDateTimeString()
        {
            return datetime.ToString("dd MMM yyyy HH:mm:ss");
        }
    }
}
