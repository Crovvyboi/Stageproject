using AnalyseApp_it._2.Data;
using AnalyseApp_it._2.Data.DataModels;
using AnalyseApp_it._2.IData;
using AnalyseApp_it._2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AnalyseApp_it._2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IDBHandler dBHandler = new DBHandler();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            
            List<TaakDMO> taakDMOs = dBHandler.GetAllTaken();

            List<TaakModel> taakModels = new List<TaakModel>();
            foreach (TaakDMO item in taakDMOs)
            {
                taakModels.Add(new TaakModel(item));
            }

            Console.WriteLine("Collected Data!");

            return View(taakModels);
        }

        public IActionResult Details(string taakId) 
        {
            Console.WriteLine("Called for a detail for taak: " + taakId);

            TaakDMO taakDMO = dBHandler.GetTaakOnId(taakId);
            TaakModel taakModel = new TaakModel(taakDMO);

            return View(taakModel);
        }

        public IActionResult Overzichten()
        {
            Console.WriteLine("Going to overzichten");

            List<OverzichtDMO> overzichtDMOs = dBHandler.GetOverzichten();

            List<OverzichtModel> overzichten = new List<OverzichtModel>();
            foreach (OverzichtDMO overzichtDMO in overzichtDMOs)
            {
                overzichten.Add(new OverzichtModel(overzichtDMO));
            }

            return View(overzichten);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
