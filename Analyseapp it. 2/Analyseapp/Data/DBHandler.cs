using Analyseapp.Data.Datamodels;
using Analyseapp.IData;
using Analyseapp.Models;
using Analyseapp.Util;
using Humanizer;
using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using Npgsql;
using NuGet.Packaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Analyseapp.Data
{
    public class DBHandler : IDBHandler
    {
        private IConfiguration _configuration;

        public DBHandler(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        // Tests ___________________________________________________________________________
        public List<TestDMO> GetAllTests()
        {
            List<TestDMO> tests = new List<TestDMO>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From test " +
                "Left Join programma on \"ProgrammaID\" = \"programmaID\" " +
                "Left Join uitvoertijd on test.\"UitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" ", 
                conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                // Get all entry data
                int testID = Convert.ToInt32(reader["TestID"]);
                string testNaam = reader["TestNaam"].ToString();
                string testBeschr = reader["TestBeschrijving"].ToString();

                Enum.TryParse(reader["Slagingsuitkomst"].ToString(), out SlagingsUitkomst slagingsuitkomst);

                ProgrammaModel programma = new ProgrammaModel(
                        Convert.ToInt32(reader["programmaID"]),
                        reader["programmaNaam"].ToString(),
                        reader["versie"].ToString(),
                        (bool)reader["isInDevelopment"]
                    );

                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];
                List<UitvoertijdModel> uitvoertijden = new List<UitvoertijdModel>();
                uitvoertijden.Add(new UitvoertijdModel(
                    uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd));

                // Get subtests
                List<TestModel> testModels = GetSubtests(testID);

                TestDMO testDMO = new TestDMO(testID, testNaam, testBeschr, programma, testModels, slagingsuitkomst, uitvoertijden[0]);

                tests.Add(testDMO);
            }

            conn.Close();

            return tests;
        }

        private List<TestModel> GetSubtests(int maintest)
        {
            List<TestModel> subtests = new List<TestModel>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From subtests Where \"MaintestID\" = " + maintest, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                TestDMO test = GetTestByID(Convert.ToInt32(reader["SubtestID"]));
                TestModel testmodel = new TestModel(test);
                subtests.Add(testmodel);
            }

            conn.Close();

            return subtests;
        }

        private List<UitvoertijdModel> GetUitvoertijden(int testid)
        {
            List<UitvoertijdModel> uitvoertijden = new List<UitvoertijdModel>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From uitvoertijd Where \"Test\" =" + testid, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];


                UitvoertijdModel uitvoertijd = new UitvoertijdModel(uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd);
                uitvoertijden.Add(uitvoertijd);
            }

            conn.Close();

            return uitvoertijden.OrderBy(x => x.uitvoertijdID).ToList();
        }

        public TestDMO GetTestByID(int testid)
        {
            TestDMO testDMO = new TestDMO();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From test " +
                "Full Join programma on \"ProgrammaID\" = \"programmaID\" " +
                "Full Join uitvoertijd on test.\"UitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" " +
                "Where \"TestID\" = " + testid, 
                conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                // Get all entry data
                string testNaam = reader["TestNaam"].ToString();
                string testBeschr = reader["TestBeschrijving"].ToString();


                ProgrammaModel programma = new ProgrammaModel(
                        Convert.ToInt32(reader["programmaID"]),
                        reader["programmaNaam"].ToString(),
                        reader["versie"].ToString(),
                        (bool)reader["isInDevelopment"]
                    );

                Enum.TryParse(reader["Slagingsuitkomst"].ToString(), out SlagingsUitkomst slagingsuitkomst);

                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];
                List<UitvoertijdModel> uitvoertijden = new List<UitvoertijdModel>();
                uitvoertijden.Add(new UitvoertijdModel(
                    uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd));

                // Get subtests
                List<TestModel> testModels = GetSubtests(testid);

                testDMO = new TestDMO(testid, testNaam, testBeschr, programma, testModels, slagingsuitkomst, uitvoertijden[0]);
                
            }

            conn.Close();

            return testDMO;
        }

        public List<UitvoertijdDMO> GetAllUitvoertijdenOnTest(string testnaam)
        {
            List<UitvoertijdDMO> uitvoertijdDMOs = new List<UitvoertijdDMO>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From test " +
                "Join uitvoertijd On test.\"UitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" " +
                "Where \"TestNaam\" = '" + testnaam + "'", 
                conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];


                UitvoertijdDMO uitvoertijd = new UitvoertijdDMO(uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd);
                uitvoertijdDMOs.Add(uitvoertijd);
            }

            conn.Close();


            return uitvoertijdDMOs;
        }
        public List<TestDMO> GetTestsAtProgramma(int progId)
        {
            List<TestDMO> tests = new List<TestDMO>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From test " +
                "Inner Join programma On test.\"ProgrammaID\" = programma.\"programmaID\" " +
                "Inner Join uitvoertijd On test.\"UitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" " +
                "Where test.\"ProgrammaID\" = " + progId,
                conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                // Get all entry data
                int testID = Convert.ToInt32(reader["TestID"]);
                string testNaam = reader["TestNaam"].ToString();
                string testBeschr = reader["TestBeschrijving"].ToString();

                Enum.TryParse(reader["Slagingsuitkomst"].ToString(), out SlagingsUitkomst slagingsuitkomst);

                ProgrammaModel programma = new ProgrammaModel(
                        Convert.ToInt32(reader["programmaID"]),
                        reader["programmaNaam"].ToString(),
                        reader["versie"].ToString(),
                        (bool)reader["isInDevelopment"]
                    );

                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];
                List<UitvoertijdModel> uitvoertijden = new List<UitvoertijdModel>();
                uitvoertijden.Add(new UitvoertijdModel(
                    uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd));

                // Get subtests
                List<TestModel> testModels = GetSubtests(testID);

                TestDMO testDMO = new TestDMO(testID, testNaam, testBeschr, programma, testModels, slagingsuitkomst, uitvoertijden[0]);

                tests.Add(testDMO);
            }

            conn.Close();

            return tests;
        }

        public int GetTestIDOnDate(DateOnly date, TimeOnly time, string testname)
        {
            int testID = 0;

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select test.\"TestID\" From test " +
                "Join uitvoertijd On test.\"UitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" " +
                "Where \"TestNaam\" = '" + testname + "' AND \"Startdatum\" = '" + date + "' AND \"Starttijd\" = '" + time + "'", 
                conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                testID = Convert.ToInt32(reader["TestID"]);
            }

            conn.Close();

            return testID;
        }

        public int? GetMaintestOnSubtest(int testid)
        {
            int? maintest = null;

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select \"MaintestID\" From subtests Where \"SubtestID\" = " + testid, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    maintest = Convert.ToInt32(reader["MaintestID"]);
                }
            }

            conn.Close();

            return maintest;
        }

        public bool GetRapportOnTest(int progid)
        {
            bool hasRapport = false;

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From rapport Where \"ProgrammaID\" = " + progid, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            if (reader.HasRows)
            {
                hasRapport = true;
            }

            conn.Close();

            return hasRapport;
        }


        // Taken ___________________________________________________________________________
        public List<TaakDMO> GetAllTaken()
        {
            List<TaakDMO> taken = new List<TaakDMO>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From taak " +
                "Inner Join programma On taak.\"programmaID\" = programma.\"programmaID\" " +
                "Inner Join uitvoertijd On taak.\"uitvoertijdID\" = uitvoertijd.\"UitvoertijdID\"", conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int taakID = Convert.ToInt32(reader["taakID"]);
                string taaknaam = reader["taakNaam"].ToString();

                string taakMessage = reader["taakMessage"].ToString();

                ProgrammaModel programma = new ProgrammaModel(
                        Convert.ToInt32(reader["programmaID"]),
                        reader["programmaNaam"].ToString(),
                        reader["versie"].ToString(),
                        (bool)reader["isInDevelopment"]
                    );

                List<TaakModel> subtaken = Subtaken(taakID);

                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];


                UitvoertijdModel uitvoertijd = new UitvoertijdModel(uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd);

                TaakDMO taakDMO = new TaakDMO(taakID, taaknaam, programma, taakMessage, subtaken, uitvoertijd);
                taken.Add(taakDMO);
            }

            conn.Close();

            return taken;
        }

        private List<TaakModel> Subtaken(int taakID)
        {
            List<TaakModel> taken = new List<TaakModel>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From subtaak Where \"maintaakID\" = " + taakID, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                TaakDMO taakdmo = GetTaakAtID(Convert.ToInt32(reader["subtaakID"]));
                TaakModel taak = new TaakModel(taakdmo);
                taken.Add(taak);
            }

            conn.Close();


            return taken;
        }

        public TaakDMO GetTaakAtID(int taakID)
        {
            TaakDMO taak = new TaakDMO();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From taak " +
                "Inner Join programma On taak.\"programmaID\" = programma.\"programmaID\" " +
                "Inner Join uitvoertijd On taak.\"uitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" " +
                "Where taak.\"taakID\" = " + taakID, 
                conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                string taaknaam = reader["taakNaam"].ToString();
                string taakMessage = reader["taakMessage"].ToString();

                ProgrammaModel programma = new ProgrammaModel(
                    Convert.ToInt32(reader["programmaID"]),
                    reader["programmaNaam"].ToString(),
                    reader["versie"].ToString(),
                    (bool)reader["isInDevelopment"]
                );

                List<TaakModel> subtaken = Subtaken(taakID);

                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];


                UitvoertijdModel uitvoertijd = new UitvoertijdModel(uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd);

                taak = new TaakDMO(taakID, taaknaam, programma, taakMessage, subtaken, uitvoertijd);
            }

            conn.Close();


            return taak;
        }
        public List<TaakDMO> GetTakenAtProgramma(int progId)
        {
            List<TaakDMO> taken = new List<TaakDMO>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From taak " +
                "Inner Join programma On taak.\"programmaID\" = programma.\"programmaID\" " +
                "Inner Join uitvoertijd On taak.\"uitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" " +
                "Where taak.\"programmaID\" = " + progId, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int taakID = Convert.ToInt32(reader["taakID"]);
                string taaknaam = reader["taakNaam"].ToString();

                string taakMessage = reader["taakMessage"].ToString();

                ProgrammaModel programma = new ProgrammaModel(
                        Convert.ToInt32(reader["programmaID"]),
                        reader["programmaNaam"].ToString(),
                        reader["versie"].ToString(),
                        (bool)reader["isInDevelopment"]
                    );

                List<TaakModel> subtaken = Subtaken(taakID);

                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];


                UitvoertijdModel uitvoertijd = new UitvoertijdModel(uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd);

                TaakDMO taakDMO = new TaakDMO(taakID, taaknaam, programma, taakMessage, subtaken, uitvoertijd);
                taken.Add(taakDMO);
            }

            conn.Close();

            return taken;
        }

        public List<UitvoertijdDMO> GetAllUitvoertijdenOnTaak(string taaknaam)
        {
            List<UitvoertijdDMO> uitvoertijdDMOs = new List<UitvoertijdDMO>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From taak " +
                "Join uitvoertijd On taak.\"uitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" " +
                "Where \"taakNaam\" = '" + taaknaam + "'",
                conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int uitvtijdID = Convert.ToInt32(reader["UitvoertijdID"]);
                DateOnly startdatum = DateOnly.FromDateTime((DateTime)reader["Startdatum"]);
                TimeOnly starttijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Starttijd"]);
                DateOnly einddatum = DateOnly.FromDateTime((DateTime)reader["Einddatum"]);
                TimeOnly eindtijd = TimeOnly.FromTimeSpan((TimeSpan)reader["Eindtijd"]);
                TimeSpan totuitvoertijd = (TimeSpan)reader["TotaleUitvoertijd"];


                UitvoertijdDMO uitvoertijd = new UitvoertijdDMO(uitvtijdID, startdatum, starttijd, einddatum, eindtijd, totuitvoertijd);
                uitvoertijdDMOs.Add(uitvoertijd);
            }

            conn.Close();


            return uitvoertijdDMOs;
        }

        public int GetTaakIDOnDate(DateOnly date, TimeOnly time, string taaknaam)
        {
            int taakid = 0;

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select taak.\"taakID\" From taak " +
                "Join uitvoertijd On taak.\"uitvoertijdID\" = uitvoertijd.\"UitvoertijdID\" " +
                "Where \"taakNaam\" = '" + taaknaam + "' AND \"Startdatum\" = '" + date + "' AND \"Starttijd\" = '" + time + "'",
                conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                taakid = Convert.ToInt32(reader["taakID"]);
            }

            conn.Close();

            return taakid;
        }

        public int? GetMaintaakOnSubtaak(int taakID)
        {
            int? maintaak = null;

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select \"maintaakID\" From subtaak Where \"subtaakID\" = " + taakID, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    maintaak = Convert.ToInt32(reader["maintaakID"]);
                }
            }

            conn.Close();

            return maintaak;
        }





        // Rapporten ___________________________________________________________________________
        public List<RapportDMO> GetRapporten()
        {
            List<RapportDMO> rapportDMOs = new List<RapportDMO>();


            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From rapport " +
                "Left Join trend On rapport.\"TrendID\" = trend.\"TrendID\"" +
                "Left Join programma On rapport.\"ProgrammaID\" = programma.\"programmaID\"", conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int rapportID = Convert.ToInt32(reader["RapportID"]);
                int testKerenUitgevoerd = Convert.ToInt32(reader["TestKerenUitgevoerd"]);
                int testSlagingskans = Convert.ToInt32(reader["TestSlagingskans"]);
                int taakKerenUitgevoerd = Convert.ToInt32(reader["TaakKerenUitgevoerd"]);
                DateOnly madeon = DateOnly.FromDateTime((DateTime)reader["MadeOn"]);

                TimeSpan testGemUitvoertijd = (TimeSpan)reader["TestGemUitvoertijd"];
                TimeSpan taakGemUitvoertijd = (TimeSpan)reader["TaakGemUitvoertijd"];

                int trendID = Convert.ToInt32(reader["TrendID"]);
                int testvku = Convert.ToInt32(reader["TestVerschilKerenUitgevoerd"]);
                int testvsk = Convert.ToInt32(reader["TesVerschilSlagingskans"]);
                TimeSpan testvgu = TimeSpan.FromSeconds(Convert.ToInt32(reader["TestVerschilGemUitvoertijdenInSec"]));
                int taakvku = Convert.ToInt32(reader["TaakVerschilKerenUitgevoerd"]);
                TimeSpan taakvgu = TimeSpan.FromSeconds(Convert.ToInt32(reader["TaakVerschilGemUitvoertijdenInSec"]));
                TrendModel trend = new TrendModel(trendID, testvku, testvsk, testvgu, taakvku, taakvgu);

                int programmaID = Convert.ToInt32(reader["programmaID"]);
                string programmaNaam = reader["programmaNaam"].ToString();
                string progversie = reader["versie"].ToString();
                bool isindevelopment = (bool)reader["isInDevelopment"];
                ProgrammaModel programma = new ProgrammaModel(programmaID, programmaNaam, progversie, isindevelopment);

                RapportDMO rapportDMO = new RapportDMO(rapportID, testKerenUitgevoerd, testSlagingskans, taakKerenUitgevoerd, trend, programma, madeon, testGemUitvoertijd, taakGemUitvoertijd);
                rapportDMOs.Add(rapportDMO);
            }

            conn.Close();


            return rapportDMOs;
        }

        public RapportDMO GetRapport(int rapportID)
        {
            RapportDMO rapportDMO = new RapportDMO();


            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From rapport " +
                "Left Join trend On rapport.\"TrendID\" = trend.\"TrendID\"" +
                "Left Join programma On rapport.\"ProgrammaID\" = programma.\"programmaID\"" +
                "Where \"RapportID\" = " + rapportID, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int testKerenUitgevoerd = Convert.ToInt32(reader["TestKerenUitgevoerd"]);
                int testSlagingskans = Convert.ToInt32(reader["TestSlagingskans"]);
                int taakKerenUitgevoerd = Convert.ToInt32(reader["TaakKerenUitgevoerd"]);
                DateOnly madeon = DateOnly.FromDateTime((DateTime)reader["MadeOn"]);

                TimeSpan testGemUitvoertijd = (TimeSpan)reader["TestGemUitvoertijd"];
                TimeSpan taakGemUitvoertijd = (TimeSpan)reader["TaakGemUitvoertijd"];

                int trendID = Convert.ToInt32(reader["TrendID"]);
                int testvku = Convert.ToInt32(reader["TestVerschilKerenUitgevoerd"]);
                int testvsk = Convert.ToInt32(reader["TesVerschilSlagingskans"]);
                TimeSpan testvgu = TimeSpan.FromSeconds(Convert.ToInt32(reader["TestVerschilGemUitvoertijdenInSec"]));
                int taakvku = Convert.ToInt32(reader["TaakVerschilKerenUitgevoerd"]);
                TimeSpan taakvgu = TimeSpan.FromSeconds(Convert.ToInt32(reader["TaakVerschilGemUitvoertijdenInSec"]));
                TrendModel trend = new TrendModel(trendID, testvku, testvsk, testvgu, taakvku, taakvgu);

                int programmaID = Convert.ToInt32(reader["programmaID"]);
                string programmaNaam = reader["programmaNaam"].ToString();
                string progversie = reader["versie"].ToString();
                bool isindevelopment = (bool)reader["isInDevelopment"];
                ProgrammaModel programma = new ProgrammaModel(programmaID, programmaNaam, progversie, isindevelopment);

                rapportDMO = new RapportDMO(rapportID, testKerenUitgevoerd, testSlagingskans, taakKerenUitgevoerd, trend, programma, madeon, testGemUitvoertijd, taakGemUitvoertijd);
            }

            conn.Close();


            return rapportDMO;
        }
        
        public List<RapportDMO> GetRapportenOfProgramma(int programmaid)
        {
            List<RapportDMO> rapportDMOs = new List<RapportDMO>();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From rapport " +
                "Left Join trend On rapport.\"TrendID\" = trend.\"TrendID\"" +
                "Left Join programma On rapport.\"ProgrammaID\" = programma.\"programmaID\"" +
                "Where \"ProgrammaID\" =" + programmaid, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int rapportID = Convert.ToInt32(reader["RapportID"]);
                int testKerenUitgevoerd = Convert.ToInt32(reader["TestKerenUitgevoerd"]);
                int testSlagingskans = Convert.ToInt32(reader["TestSlagingskans"]);
                int taakKerenUitgevoerd = Convert.ToInt32(reader["TaakKerenUitgevoerd"]);
                DateOnly madeon = DateOnly.FromDateTime((DateTime)reader["MadeOn"]);

                TimeSpan testGemUitvoertijd = (TimeSpan)reader["TestGemUitvoertijd"];
                TimeSpan taakGemUitvoertijd = (TimeSpan)reader["TaakGemUitvoertijd"];


                int trendID = Convert.ToInt32(reader["TrendID"]);
                int testvku = Convert.ToInt32(reader["TestVerschilKerenUitgevoerd"]);
                int testvsk = Convert.ToInt32(reader["TesVerschilSlagingskans"]);
                TimeSpan testvgu = TimeSpan.FromSeconds(Convert.ToInt32(reader["TestVerschilGemUitvoertijdenInSec"]));
                int taakvku = Convert.ToInt32(reader["TaakVerschilKerenUitgevoerd"]);
                TimeSpan taakvgu = TimeSpan.FromSeconds(Convert.ToInt32(reader["TaakVerschilGemUitvoertijdenInSec"]));
                TrendModel trend = new TrendModel(trendID, testvku, testvsk, testvgu, taakvku, taakvgu);

                int programmaID = Convert.ToInt32(reader["programmaID"]);
                string programmaNaam = reader["programmaNaam"].ToString();
                string progversie = reader["versie"].ToString();
                bool isindevelopment = (bool)reader["isInDevelopment"];
                ProgrammaModel programma = new ProgrammaModel(programmaID, programmaNaam, progversie, isindevelopment);


                RapportDMO rapportDMO = new RapportDMO(rapportID, testKerenUitgevoerd, testSlagingskans, taakKerenUitgevoerd, trend, programma, madeon, testGemUitvoertijd, taakGemUitvoertijd);
                rapportDMOs.Add(rapportDMO);
            }

            conn.Close();

            return rapportDMOs;
        }

        private TrendModel GetTrendAtID(int trendID)
        {
            TrendModel trend = new TrendModel();

            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From trend Where \"TrendID\" = " + trendID, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int testvku = Convert.ToInt32(reader["TestVerschilKerenUitgevoerd"]);
                int testvsk = Convert.ToInt32(reader["TesVerschilSlagingskans"]);
                TimeSpan testvgu = (TimeSpan)reader["TestVerschilGemUitvoertijden"];
                int taakvku = Convert.ToInt32(reader["TaakVerschilKerenUitgevoerd"]);
                TimeSpan taakvgu = (TimeSpan)reader["TaakVerschilGemUitvoertijden"];

                TimeSpan testVGemUitvoertijden = TimeSpan.FromSeconds(Convert.ToInt32(reader["TestVerschilGemUitvoertijdenInSec"]));
                TimeSpan taakVGemUitvoertijden = TimeSpan.FromSeconds(Convert.ToInt32(reader["TaakVerschilGemUitvoertijdenInSec"]));

                trend = new TrendModel(trendID, testvku, testvsk, testvgu, taakvku, taakvgu);
            }

            conn.Close();

            return trend;
        }
        public List<ProgrammaDMO> GetProgrammas()
        {
            List<ProgrammaDMO> programmaDMOs = new List<ProgrammaDMO>();


            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From programma ", conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int programmaid = Convert.ToInt32(reader["programmaID"]);
                string programmaName = reader["programmaNaam"].ToString();
                string versie = reader["versie"].ToString();
                bool isInDevelopment = (bool)reader["isInDevelopment"];

                ProgrammaDMO programmaDMO = new ProgrammaDMO(programmaid, programmaName, versie, isInDevelopment);
                programmaDMOs.Add(programmaDMO);
            }

            conn.Close();


            return programmaDMOs;
        }

        public ProgrammaDMO GetProgramma(int programmaID)
        {
            ProgrammaDMO programmaDMO = new ProgrammaDMO();


            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("Select * From programma Where \"programmaID\" = " + programmaID, conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            // Process result stream
            while (reader.Read())
            {
                int programmaid = Convert.ToInt32(reader["programmaID"]);
                string programmaName = reader["programmaNaam"].ToString();
                string versie = reader["versie"].ToString();
                bool isInDevelopment = (bool)reader["isInDevelopment"];

                programmaDMO = new ProgrammaDMO(programmaid, programmaName, versie, isInDevelopment);
            }

            conn.Close();


            return programmaDMO;
        }


        // Write Rapport _______________________________________________________________________
        public int InsertRapport(RapportModel rapport)
        {


            return -1;
        }

        public int InsertTrend(TrendModel trend)
        {
            // Make db connection
            string connString = _configuration.GetConnectionString("DefaultConnection");
            using NpgsqlConnection conn = new NpgsqlConnection(connString);

            // Open connection
            conn.Open();

            // Setup query
            using NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO trend(" +
                "\"TestVerschilKerenUitgevoerd\", " +
                "\"TesVerschilSlagingskans\", " +
                "\"TaakVerschilKerenUitgevoerd\", " +
                "\"TestVerschilGemUitvoertijdenInSec\", " +
                "\"TaakVerschilGemUitvoertijdenInSec\") VALUES(" +
                trend.testVerschilKerenUitgevoerd + ", " +
                trend.testVerschilSlagingskans + ", " +
                trend.taakVerschilKerenUitgevoerd + ", " +
                Convert.ToInt32(trend.testVerschilGemUitvoertijden.TotalSeconds) + ", " +
                Convert.ToInt32(trend.taakVerschilGemUitvoertijden.TotalSeconds) + "); ", conn);

            // Execute query
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            conn.Close();

            conn.Open();
            int trendid = -1;
            using NpgsqlCommand cmd2 = new NpgsqlCommand("Select \"TrendID\" From trend " +
                "Where \"TestVerschilKerenUitgevoerd\" = " + trend.testVerschilKerenUitgevoerd + " " +
                "AND \"TesVerschilSlagingskans\" = " + trend.testVerschilSlagingskans + " " +
                "AND \"TaakVerschilKerenUitgevoerd\" = " + trend.taakVerschilKerenUitgevoerd + " " +
                "AND \"TestVerschilGemUitvoertijdenInSec\" = " + Convert.ToInt32(trend.testVerschilGemUitvoertijden.TotalSeconds) + " " +
                "AND \"TaakVerschilGemUitvoertijdenInSec\" = " + Convert.ToInt32(trend.taakVerschilGemUitvoertijden.TotalSeconds) + " " , conn);
            using NpgsqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                trendid = Convert.ToInt32(reader2["TrendID"]);
            }

            conn.Close();


            return trendid;
        }
    }
}
