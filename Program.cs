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
                Console.WriteLine("Usage: WiFiMapDataImporter <mongoUrl (i.e. mongodb://localhost:27017)> <jsonFilePath1> (<jsonFilePath2> ...)");
                return;
            }

            Network[] networks = JsonParser.ParseWiFiMapData(args[1..]);
            
            var mongoUrl = args[0];
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