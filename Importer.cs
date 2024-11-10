using System;
using MongoDB.Driver;
using WiFiMapDataImporter.DTO;

namespace WiFiMapDataImporter;

public class Importer(string mongoUrl)
{
    private readonly MongoClient _client = new(mongoUrl);

    public class ImportOptions
    {
        public string Database { get; init; }
        public string Collection { get; init; }
    }

    public void ImportNetworks(Network[] networks, ImportOptions options)
    {
        var database = _client.GetDatabase(options.Database);
        var collection = database.GetCollection<Network>(options.Collection);
        
        foreach (var network in networks)
        {
            try
            {
                var filter = Builders<Network>.Filter.Eq(doc => doc.Id, network.Id);
                var result = collection.ReplaceOne(
                    filter: filter,
                    replacement: network,
                    options: new ReplaceOptions { IsUpsert = true }
                );

                if (result.MatchedCount > 0)
                {
                    Logger.LogUpdate($"Network '{network.Bssid}' updated.");
                }
                else
                {
                    Logger.LogAdd($"A new network '{network.Bssid}' created.");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }
    }
}