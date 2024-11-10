using System.Collections.Generic;
using System.IO;
using WiFiMapDataImporter.DTO;
using Newtonsoft.Json.Linq;

namespace WiFiMapDataImporter;

public static class JsonParser
{
    public static Network[] DeserializeWiFiMapData(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var jsonObject = JObject.Parse(json);
        var networks = new List<Network>();

        foreach (var networkEntry in jsonObject)
        {
            var network = new Network(networkEntry.Key)
            {
                Bssid = networkEntry.Key,
                Ssid = networkEntry.Value!["ssid"]?.ToString(),
                Frequency = networkEntry.Value!["frequency"]!.ToObject<int>(),
                LastUpdateTime = networkEntry.Value["lastUpdateTime"]!.ToObject<long>(),
            };

            var levels = new List<Network.Level>();
            if (networkEntry.Value["levels"] is JObject levelsObject)
            {
                foreach (var levelEntry in levelsObject)
                {
                    var coordinates = levelEntry.Key.Split(',');
                    if (coordinates.Length == 2 &&
                        double.TryParse(coordinates[0], out var latitude) &&
                        double.TryParse(coordinates[1], out var longitude))
                    {
                        levels.Add(new Network.Level
                        {
                            Latitude = latitude,
                            Longitude = longitude,
                            Power = levelEntry.Value.ToObject<int>()
                        });
                    }
                }
            }
            network.Levels = levels.ToArray();
            networks.Add(network);
        }
        
        return networks.ToArray();
    }
}