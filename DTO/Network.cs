using System;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WiFiMapDataImporter.DTO;

public class Network(string bssid)
{
    public class Level
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int Power { get; init; }
    }

    [BsonId] public ObjectId Id { get; private set; } = GenerateObjectIdFromBssid(bssid);
    public string Bssid { get; set; }
    public string Ssid { get; set; }
    public int Frequency { get; set; }
    public string Capabilities { get; set; }

    private static ObjectId GenerateObjectIdFromBssid(string bssid)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(bssid));
        byte[] objectIdBytes = new byte[12];
        Buffer.BlockCopy(hash, 0, objectIdBytes, 0, 12);
        return new ObjectId(objectIdBytes);
    }

    private Level[] _levels;

    [BsonIgnore]
    public Level[] Levels
    {
        get => _levels;
        set
        {
            _levels = value;
            var approximateLocation = CalculateApproximateLocation();
            Latitude = approximateLocation.latitude;
            Longitude = approximateLocation.lognitude;
        }
    }

    [BsonElement] 
    private double Latitude { get; set; }
    
    [BsonElement] 
    public double Longitude { get; set; }

    private (double latitude, double lognitude) CalculateApproximateLocation()
    {
        if (_levels == null || _levels.Length == 0)
        {
            return (0,0);
        }

        double totalWeight = 0;
        double latitudeSum = 0;
        double longitudeSum = 0;

        foreach (var level in _levels)
        {
            double weight = 1.0 / Math.Abs(level.Power);
            latitudeSum += level.Latitude * weight;
            longitudeSum += level.Longitude * weight;
            totalWeight += weight;
        }

        return (latitudeSum / totalWeight, longitudeSum / totalWeight);
    }
}