using Analyseapp.Data;
using Analyseapp.Data.Datamodels;
using Analyseapp.IData;
using Analyseapp.Models;
using Analyseapp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Analyseapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _configuration;

        IDBHandler dBHandler;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            dBHandler = new DBHandler(_configuration);
        }


        // Test -------------------------------------------------------------------------------------------
        public IActionResult Index()
        {
            List<TestDMO> testDMOs = dBHandler.GetAllTests();
            List<TestModel> testModels = new List<TestModel>();
            foreach (TestDMO testDMO in testDMOs)
            {
                TestModel testModel = new TestModel(testDMO);
                testModels.Add(testModel);
            }
            
            testModels = testModels.OrderByDescending(t => t.uitvoertijd.startDatum).ThenByDescending(x => x.uitvoertijd.startTijd).GroupBy(x => x.testNaam).Select(x => x.First()).ToList();

            return View(
                    testModels
                );
        }

        public IActionResult Details(int testID)
        {
            TestDetailViewModel model = MakeTestViewModel(testID);
            return View(model);
        }

        [ActionName("DetailsWithDate")]
        public IActionResult Details(DateOnly date, TimeOnly time, string testname)
        {
            int id = dBHandler.GetTestIDOnDate(date, time, testname);

            return RedirectToAction("Details", "Home", new {testID = id} );
        }


        public TestDetailViewModel MakeTestViewModel(int testID)
        {
            // Get this test
            TestDMO testDMO = dBHandler.GetTestByID(testID);
            TestModel testModel = new TestModel(testDMO);

            // Get all uitvoertijden of test
            List<UitvoertijdDMO> uitvoertijdDMOs = dBHandler.GetAllUitvoertijdenOnTest(testModel.testNaam);
            List<UitvoertijdModel> uitvoertijden = new List<UitvoertijdModel>();
            foreach (UitvoertijdDMO uitvoertijdDMO in uitvoertijdDMOs)
            {
                UitvoertijdModel uitvoertijd = new UitvoertijdModel(uitvoertijdDMO);
                uitvoertijden.Add(uitvoertijd);
            }
            uitvoertijden = uitvoertijden.OrderByDescending(x => x.startDatum).ThenByDescending(x => x.startTijd).ToList();

            // Get parent test
            int? maintest = dBHandler.GetMaintestOnSubtest(testID);

            // Get uitvoeringen count of last 7 days
            List<UitvoertijdModel> uitvLast7Days = uitvoertijden.FindAll(
                x => testModel.uitvoertijd.startDatum.ToDateTime(testModel.uitvoertijd.startTijd).Subtract(x.startDatum.ToDateTime(x.startTijd)).TotalDays <= 7 &&
               testModel.uitvoertijd.startDatum.ToDateTime(testModel.uitvoertijd.startTijd).Subtract(x.startDatum.ToDateTime(x.startTijd)).TotalDays >= 0).ToList();

            // Get slagingspercentage
            List<TestModel> slagingsmodels = new List<TestModel>();
            double slagingspercentage = 0;
            foreach (UitvoertijdModel item in uitvLast7Days)
            {
                int itemid = dBHandler.GetTestIDOnDate(item.startDatum, item.startTijd, testModel.testNaam);
                TestDMO testdmo = dBHandler.GetTestByID(itemid);
                TestModel test = new TestModel(testdmo);
                slagingsmodels.Add(test);
            }
            if (slagingsmodels.Count > 0)
            {
                slagingspercentage = (slagingsmodels.FindAll(x => x.slagingsUitkomst == Util.SlagingsUitkomst.Geslaagd).Count * 100) / slagingsmodels.Count();
            }


            // Get gem uitvoertijd
            TimeSpan uitvtijdSum = new TimeSpan(0, 0, 0);
            foreach (UitvoertijdModel item in uitvLast7Days)
            {
                uitvtijdSum += item.totUitvoertijd;
            }
            if (uitvtijdSum.CompareTo(new TimeSpan(0, 0, 0)) > 0 && uitvLast7Days.Count() > 0)
            {
                uitvtijdSum = uitvtijdSum.Divide(uitvLast7Days.Count());
            }


            // Trend
            DateOnly trendDate;
            TimeOnly trendTime;

            int trentKeeruitgevoerd = 0;
            double trendPercentage = 0;
            TimeSpan trendUitvsom = new TimeSpan(0, 0, 0);
            if (testModel.uitvoertijd.startDatum.Equals(new DateOnly(0001, 1, 1)))
            {
            }
            else
            {
                trendDate = testModel.uitvoertijd.startDatum.AddDays(-1);
                trendTime = testModel.uitvoertijd.startTijd;

                List<UitvoertijdModel> trendLast7Days = uitvoertijden.FindAll(
                     x => trendDate.ToDateTime(trendTime).Subtract(x.startDatum.ToDateTime(x.startTijd)).TotalDays <= 7 &&
                    trendDate.ToDateTime(trendTime).Subtract(x.startDatum.ToDateTime(x.startTijd)).TotalDays >= 0).ToList();
                trentKeeruitgevoerd = uitvLast7Days.Count() - trendLast7Days.Count();

                List<TestModel> trendSlagingsmodels = new List<TestModel>();

                foreach (UitvoertijdModel item in trendLast7Days)
                {
                    int itemid = dBHandler.GetTestIDOnDate(item.startDatum, item.startTijd, testModel.testNaam);
                    TestDMO testdmo = dBHandler.GetTestByID(itemid);
                    TestModel test = new TestModel(testdmo);
                    trendSlagingsmodels.Add(test);
                }
                if (trendSlagingsmodels.Count > 0)
                {
                    trendPercentage = (trendSlagingsmodels.FindAll(x => x.slagingsUitkomst == Util.SlagingsUitkomst.Geslaagd).Count * 100)
                        / trendSlagingsmodels.Count();
                }
                trendPercentage = slagingspercentage - trendPercentage;

                foreach (UitvoertijdModel item in trendLast7Days)
                {
                    trendUitvsom += item.totUitvoertijd;
                }
                if (trendUitvsom.CompareTo(new TimeSpan(0, 0, 0)) > 0 && trendLast7Days.Count() > 0)
                {
                    trendUitvsom = trendUitvsom.Divide(trendLast7Days.Count());
                }
            }

            TestDetailViewModel dvm = new TestDetailViewModel(
                testModel,
                uitvoertijden,
                maintest,
                slagingspercentage,
                uitvLast7Days.Count(),
                uitvtijdSum,
                new TrendModel(0, trentKeeruitgevoerd, trendPercentage, trendUitvsom, 0, new TimeSpan())
                );
            return dvm;
        }

        // Taak -------------------------------------------------------------------------------------------
        public IActionResult TaakOverview()
        {
            List<TaakModel> taken = new List<TaakModel>();
            List<TaakDMO> taakDMOs = dBHandler.GetAllTaken();
            foreach (TaakDMO item in taakDMOs)
            {
                TaakModel taak = new TaakModel(item);
                taken.Add(taak);
            }
            taken = taken.OrderByDescending(x => x.uitvoertijd.startDatum).ThenByDescending(x => x.uitvoertijd.startTijd).GroupBy(x => x.taakName).Select(x => x.First()).ToList();

            return View(taken);
        }

        public IActionResult TaakDetail(int taakID)
        {
            TaakDetailViewModel model =  MakeTaakViewModel(taakID);
            return View(model);
        }

        [ActionName("TaakDetailsWithDate")]
        public IActionResult TaakDetail(DateOnly date, TimeOnly time, string taakname)
        {
            int id = dBHandler.GetTaakIDOnDate(date, time, taakname);

            return RedirectToAction("TaakDetail", "Home", new { taakID = id });
        }


        public TaakDetailViewModel MakeTaakViewModel(int taakID)
        {
            TaakDMO taakDMO = dBHandler.GetTaakAtID(taakID);
            TaakModel taak = new TaakModel(taakDMO);

            List<UitvoertijdDMO> uitvoertijdDMOs = dBHandler.GetAllUitvoertijdenOnTaak(taak.taakName);
            List<UitvoertijdModel> uitvoertijden = new List<UitvoertijdModel>();
            foreach (UitvoertijdDMO uitvoertijdDMO in uitvoertijdDMOs)
            {
                UitvoertijdModel uitvoertijd = new UitvoertijdModel(uitvoertijdDMO);
                uitvoertijden.Add(uitvoertijd);
            }

            // Get parent taak
            int? maintaak = dBHandler.GetMaintaakOnSubtaak(taakID);

            // Get uitvoeringen count of last 7 days
            List<UitvoertijdModel> uitvLast7Days = uitvoertijden.FindAll(
                x => taak.uitvoertijd.startDatum.ToDateTime(taak.uitvoertijd.startTijd).Subtract(x.startDatum.ToDateTime(x.startTijd)).TotalDays <= 7 &&
               taak.uitvoertijd.startDatum.ToDateTime(taak.uitvoertijd.startTijd).Subtract(x.startDatum.ToDateTime(x.startTijd)).TotalDays >= 0).ToList();

            TimeSpan uitvtijdSum = new TimeSpan(0, 0, 0);
            foreach (UitvoertijdModel item in uitvLast7Days)
            {
                uitvtijdSum += item.totUitvoertijd;
            }
            if (uitvtijdSum.CompareTo(new TimeSpan(0, 0, 0)) > 0 && uitvLast7Days.Count() > 0)
            {
                uitvtijdSum = uitvtijdSum.Divide(uitvLast7Days.Count());
            }

            // Trend
            DateOnly trendDate;
            TimeOnly trendTime;

            int trentKeeruitgevoerd = 0;
            TimeSpan trendUitvsom = new TimeSpan(0, 0, 0);
            if (taak.uitvoertijd.startDatum.Equals(new DateOnly(0001, 1, 1)))
            {
            }
            else
            {
                trendDate = taak.uitvoertijd.startDatum.AddDays(-1);
                trendTime = taak.uitvoertijd.startTijd;

                List<UitvoertijdModel> trendLast7Days = uitvoertijden.FindAll(
                     x => trendDate.ToDateTime(trendTime).Subtract(x.startDatum.ToDateTime(x.startTijd)).TotalDays <= 7 &&
                    trendDate.ToDateTime(trendTime).Subtract(x.startDatum.ToDateTime(x.startTijd)).TotalDays >= 0).ToList();
                trentKeeruitgevoerd = uitvLast7Days.Count() - trendLast7Days.Count();

                foreach (UitvoertijdModel item in trendLast7Days)
                {
                    trendUitvsom += item.totUitvoertijd;
                }
                if (trendUitvsom.CompareTo(new TimeSpan(0, 0, 0)) > 0 && trendLast7Days.Count() > 0)
                {
                    trendUitvsom = trendUitvsom.Divide(trendLast7Days.Count());
                }
            }

            TaakDetailViewModel model = new TaakDetailViewModel(
                taak,
                uitvoertijden,
                maintaak,
                uitvtijdSum,
                uitvLast7Days.Count(),
                trentKeeruitgevoerd,
                trendUitvsom);
            return model;
        }

        // Rapport -------------------------------------------------------------------------------------------
        public IActionResult RapportOverview()
        {
            List<RapportDMO> rapportDMOs = dBHandler.GetRapporten();

            List<RapportModel> rapporten = new List<RapportModel>();
            foreach (RapportDMO item in rapportDMOs)
            {
                RapportModel rapport = new RapportModel(item);
                rapporten.Add(rapport);
            }

            List<ProgrammaDMO> programmaDMOs = dBHandler.GetProgrammas();
            List<ProgrammaModel> programmas = new List<ProgrammaModel>();
            foreach (ProgrammaDMO item in programmaDMOs)
            {
                ProgrammaModel programma = new ProgrammaModel(item);
                programmas.Add(programma);
            }

            RapportOverviewViewModel rapportOverviewViewModel = new RapportOverviewViewModel(rapporten, programmas);

            return View(rapportOverviewViewModel);
        }
        
        public IActionResult Rapport(int rapportID)
        {
            // Get rapport details
            RapportDMO rapportDMO = dBHandler.GetRapport(rapportID);
            RapportModel rapport = new RapportModel(rapportDMO);

            // For each Test before date, get entries
            List<TestDMO> testDMOs = dBHandler.GetTestsAtProgramma(rapport.programma.programmaID);
            List<TestModel> tests = new List<TestModel>();
            foreach (TestDMO item in testDMOs)
            {
                if (item.uitvoertijd.startDatum.CompareTo(rapport.madeOn) <= 0)
                {
                    TestModel test = new TestModel(item);
                    tests.Add(test);
                }

            }

            // For each Taak before date, get entries
            List<TaakDMO> taakDMOs = dBHandler.GetTakenAtProgramma(rapport.programma.programmaID);
            List<TaakModel> taken = new List<TaakModel>();
            foreach (TaakDMO item  in taakDMOs)
            {
                if (item.uitvoertijd.startDatum.CompareTo(rapport.madeOn) <= 0)
                {
                    TaakModel taak = new TaakModel(item);
                    taken.Add(taak);
                }
            }


            // Form into ViewModel
            RapportViewModel model = new RapportViewModel(rapport, tests, taken);

            return View(model);
        }


        [ActionName("RapportPass")]
        public IActionResult Rapport(RapportModel rapport)
        {
            // For each Test before date, get entries
            List<TestDMO> testDMOs = dBHandler.GetTestsAtProgramma(rapport.programma.programmaID);
            List<TestModel> tests = new List<TestModel>();
            foreach (TestDMO item in testDMOs)
            {
                if (item.uitvoertijd.startDatum.CompareTo(rapport.madeOn) <= 0)
                {
                    TestModel test = new TestModel(item);
                    tests.Add(test);
                }

            }

            // For each Taak before date, get entries
            List<TaakDMO> taakDMOs = dBHandler.GetTakenAtProgramma(rapport.programma.programmaID);
            List<TaakModel> taken = new List<TaakModel>();
            foreach (TaakDMO item in taakDMOs)
            {
                if (item.uitvoertijd.startDatum.CompareTo(rapport.madeOn) <= 0)
                {
                    TaakModel taak = new TaakModel(item);
                    taken.Add(taak);
                }
            }


            // Form into ViewModel
            RapportViewModel model = new RapportViewModel(rapport, tests, taken);

            return View(model);
        }

        public IActionResult MaakRapport(int programmaid)
        {
            // Get list of each test run on programid
            List<TestDMO> testDMOs = dBHandler.GetTestsAtProgramma(programmaid);
            List<TestModel> testen = new List<TestModel>();
            foreach (TestDMO testDMO  in testDMOs)
            {
                TestModel test = new TestModel(testDMO);
                testen.Add(test);
            }

            // Get list of each taak run on programid
            List<TaakDMO> taakDMOs = dBHandler.GetTakenAtProgramma(programmaid);
            List<TaakModel> taken = new List<TaakModel>();
            foreach (TaakDMO taakDMO in taakDMOs)
            {
                TaakModel taak = new TaakModel(taakDMO);
                taken.Add(taak);
            }

            // Get programma
            ProgrammaDMO programmaDMO = dBHandler.GetProgramma(programmaid);
            ProgrammaModel programma = new ProgrammaModel(programmaDMO);

            // Calculate rapport things with these lists
            int testKerenUitgevoerd = testen.Count();
            int slagingskans = Convert.ToInt32(testKerenUitgevoerd * ((double)testen.Where(x => x.slagingsUitkomst.Equals(Util.SlagingsUitkomst.Geslaagd)).Count() / 100) * 100);
            int taakKerenUitgevoerd = taken.Count();

            TimeSpan testTotaleUitvoertijd = new TimeSpan(0,0,0);
            foreach (TestModel test in testen)
            {
                testTotaleUitvoertijd = testTotaleUitvoertijd.Add(test.uitvoertijd.totUitvoertijd);
            }

            TimeSpan taakTotaleUitvoertijd = new TimeSpan(0,0,0);
            foreach (TaakModel taak in taken) 
            {
                taakTotaleUitvoertijd = taakTotaleUitvoertijd.Add(taak.uitvoertijd.totUitvoertijd);
            }

            TimeSpan testGemUitvoertijd = testTotaleUitvoertijd.Divide(testKerenUitgevoerd);
            TimeSpan taakGemUitvoertijd = taakTotaleUitvoertijd.Divide(taakKerenUitgevoerd);

            DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);

            // Determine trends compared to last rapport of the same program
            List<RapportDMO> rapportDMO = dBHandler.GetRapportenOfProgramma(programmaid);
            RapportModel prevRapport = new RapportModel(rapportDMO.OrderBy(x => x.madeOn).Last());

            int testVerschilKerenUitgevoerd = prevRapport.testKerenUitgevoerd - testKerenUitgevoerd;
            double testVerschilSlagingskans = prevRapport.testSlagingskans - slagingskans;
            TimeSpan testVerschilGemUitvoertijden = prevRapport.testGemUitvoertijd.Subtract(testGemUitvoertijd);

            int taakVerschilKerenUitgevoerd = prevRapport.taakKerenUitgevoerd - taakKerenUitgevoerd;
            TimeSpan taakVerschilGemUitvoertijden = prevRapport.taakGemUitvoertijd.Subtract(taakGemUitvoertijd);

            // Construct Rapport
            TrendModel trend = new TrendModel(-1, testVerschilKerenUitgevoerd, testVerschilSlagingskans, testVerschilGemUitvoertijden, taakVerschilKerenUitgevoerd, taakVerschilGemUitvoertijden);
            RapportModel rapport = new RapportModel(testKerenUitgevoerd, slagingskans, taakKerenUitgevoerd, trend, programma, dateNow, testGemUitvoertijd, taakGemUitvoertijd);

            // Put rapport and trend in db
            int trendID = dBHandler.InsertTrend(trend);
            rapport.trend.SetID(trendID);



            // Get new rapportID

            return RedirectToAction("RapportPass", "Home", new { rapport = rapport });
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
