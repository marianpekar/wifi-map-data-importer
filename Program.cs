using System;
using WiFiMapDataImporter.DTO;

namespace WiFiMapDataImporter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: WiFiMapDataImporter <jsonFilePath> <mongoUrl (i.e. mongodb://localhost:27017)>");
                return;
            }

            Network[] networks = JsonParser.DeserializeWiFiMapData(args[0]);
            
            var mongoUrl = args[1];
            new Importer(mongoUrl).ImportNetworks(
                networks,
                new Importer.ImportOptions
                {
                    Database = "wifi_map", Collection = "networks"
                }
            );
        }
    }
}