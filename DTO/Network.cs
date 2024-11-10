using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WiFiMapDataImporter.DTO;

public class Network(string bssid)
{
    public class Level
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Power { get; set; }
    }
  
    [BsonId]
    public ObjectId Id { get; private set; } = GenerateObjectIdFromBssid(bssid);

    private static ObjectId GenerateObjectIdFromBssid(string bssid)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(bssid));
        var sb = new StringBuilder();
        foreach (byte b in hash)
        {
            sb.Append(b.ToString("x2"));
        }
        return new ObjectId(sb.ToString());
    }

    public string Bssid { get; set; }
    public string Ssid { get; set; }
    public int Frequency { get; set; }
    public long LastUpdateTime { get; set; }
    public Level[] Levels { get; set; }
}