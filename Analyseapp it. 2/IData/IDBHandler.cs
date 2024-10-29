using AnalyseApp_it._2.Data.DataModels;

namespace AnalyseApp_it._2.IData
{
    public interface IDBHandler
    {
        public List<TaakDMO> GetAllTaken();
        public TaakDMO GetTaakOnId(string taakID);
        public List<OverzichtDMO> GetOverzichten();
        public OverzichtDMO GetOverzichtOnNaam(string overzichtNaam);

    }
}
