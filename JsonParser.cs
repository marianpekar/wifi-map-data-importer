using System.Collections.Generic;
using System.IO;
using WiFiMapDataImporter.DTO;
using Newtonsoft.Json.Linq;

namespace WiFiMapDataImporter;

public static class JsonParser
{
    public static Network[] ParseWiFiMapData(string[] filePaths)
    {
        JObject jsonObject = ReadAndMerge(filePaths);

        var networks = new List<Network>();
        foreach (var networkEntry in jsonObject)
        {
            var network = new Network(networkEntry.Key)
            {
                Bssid = networkEntry.Key,
                Ssid = networkEntry.Value!["ssid"]?.ToString(),
                Frequency = networkEntry.Value!["frequency"]!.ToObject<int>(),
                Capabilities = networkEntry.Value["capabilities"]!.ToString(),
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

private static JObject ReadAndMerge(string[] filePaths)
    {
        string json = File.ReadAllText(filePaths[0]);
        JObject jsonObject = JObject.Parse(json);

        for (int i = 1; i < filePaths.Length; i++)
        {
            string nextJson = File.ReadAllText(filePaths[i]);
            JObject nextJsonObject = JObject.Parse(nextJson);

            foreach (var nextProperty in nextJsonObject.Properties())
            {
                string key = nextProperty.Name;
                
                if (jsonObject.TryGetValue(key, out var value))
                {
                    var currentValue = value as JObject;
                    var newValue = nextJsonObject[key] as JObject;

                    if (currentValue != null && newValue != null)
                    {
                        foreach (var newProperty in newValue.Properties())
                        {
                            if (newProperty.Value.HasValues || !string.IsNullOrEmpty(newProperty.Value.ToString()))
                            {
                                currentValue[newProperty.Name] = newProperty.Value;
                            }
                        }
                        
                        if (currentValue["levels"] is JObject existingLevels && newValue["levels"] is JObject newLevels)
                        {
                            foreach (var level in newLevels.Properties())
                            {
                                existingLevels[level.Name] = level.Value;
                            }
                        }
                    }
                }
                else
                {
                    jsonObject[key] = nextProperty.Value;
                }
            }
        }

        return jsonObject;
    }
}