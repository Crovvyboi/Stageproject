using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using System.Text.Json.Nodes;
using AnalyseApp_it._2.Models;
using AnalyseApp_it._2.Data.DataModels;
using AnalyseApp_it._2.IData;

namespace AnalyseApp_it._2.Data
{
    public class DBHandler : IDBHandler
    {

        public List<TaakDMO> GetAllTaken()
        {
            List<TaakDMO> taken = new List<TaakDMO>();
            
            // Replace the placeholder with your Atlas connection string
            const string connectionUri = "mongodb+srv://riksmolders:Mieper99-Mieper99@cluster0.z5tuv.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            // Sets the ServerApi field of the settings object to Stable API version 1
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // Creates a new client and connects to the server
            var client = new MongoClient(settings);
            // Sends a ping to confirm a successful connection
            try
            {
                var result = client.GetDatabase("analyse_data").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");

                var bson = client.GetDatabase("analyse_data").GetCollection<BsonDocument>("taken").Find(_ => true).ToList();

                foreach (var item in bson)
                {
                    // Convert Bson object to readable Json structure
                    JsonObject jsonobj = (JsonObject)JsonObject.Parse(item.ToString());
                    
                    // Get all taak data
                    JsonObject idObj = (JsonObject)jsonobj.First(x => x.Key.Contains("_id")).Value;
                    string id = (string)idObj.First(x => x.Key.Contains("$oid")).Value;

                    JsonObject js = (JsonObject)jsonobj.First(x => x.Key.Contains("taak")).Value;

                    int added_columns_count = (int)js.First(x => x.Key.Contains("added_column_count")).Value;
                    int removed_columns_count = (int)js.First(x => x.Key.Contains("removed_column_count")).Value;
                    int changes_count = (int)js.First(x => x.Key.Contains("changes_count")).Value;

                    JsonObject dateObj = (JsonObject)js.First(x => x.Key.Contains("uitvoer_date")).Value;
                    DateTimeOffset dateTime = DateTimeOffset.Parse((string)dateObj.First(x => x.Key.Contains("$date")).Value);

                    // Get subtaak object
                    List<SubtaakModel> subtaken = new List<SubtaakModel>();

                    JsonObject dataObj = (JsonObject)js.First(x => x.Key.Contains("diff_data")).Value;
                    JsonArray dataArray = (JsonArray)dataObj.First(x => x.Key.Contains("data")).Value;

                    foreach (JsonObject dataItem in dataArray)
                    {
                        string overzicht = (string)dataItem.First(x => x.Key.Contains("overzicht")).Value;
                        string added_columns = (string)dataItem.First(x => x.Key.Contains("added_columns")).Value;
                        string removed_columns = (string)dataItem.First(x => x.Key.Contains("removed_columns")).Value;

                        List<DiffModel> diffModels = new List<DiffModel>();
                        JsonArray diffArray = (JsonArray)dataItem.First(x => x.Key.Contains("found_diffs")).Value;
                        foreach (JsonObject diffObj in diffArray)
                        {
                            string onEntryId = (string)diffObj.First(x => x.Key.Contains("OnEntryID")).Value;
                            string onColumn = (string)diffObj.First(x => x.Key.Contains("OnColumn")).Value;
                            string valueA = (string)diffObj.First(x => x.Key.Contains("ValueA")).Value;
                            string valueB = (string)diffObj.First(x => x.Key.Contains("ValueB")).Value;

                            diffModels.Add(new DiffModel(onEntryId, onColumn, valueA, valueB));
                        }

                        subtaken.Add(new SubtaakModel(overzicht, added_columns, removed_columns, diffModels));
                    }

                    taken.Add(new TaakDMO(id, dateTime, added_columns_count, removed_columns_count, changes_count, subtaken));
                }
                
            }
            catch (Exception ex) { Console.WriteLine(ex); }

            return taken;
        }

    }
}
