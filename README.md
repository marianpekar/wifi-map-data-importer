## Wi-Fi Map Data Importer

A [.NET](https://dotnet.microsoft.com) 8 console application that parses the `wifi_map_data.json` output file from the *Wi-Fi Cartographer* Android app, calculates the approximate location of each AP, and stores a record for each network as a document in a [MongoDB](https://www.mongodb.com) collection where every document follows this JSON schema.

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "properties": {
    "_id": {
      "type": "object",
      "properties": {
        "$oid": {
          "type": "string",
          "pattern": "^[0-9a-fA-F]{24}$"
        }
      },
      "required": ["$oid"],
      "additionalProperties": false
    },
    "Bssid": {
      "type": "string",
      "pattern": "^[0-9a-fA-F]{2}(:[0-9a-fA-F]{2}){5}$"
    },
    "Ssid": {
      "type": "string",
    },
    "Frequency": {
      "type": "integer",
    },
    "Capabilities": {
      "type": "string",
    },
    "Longitude": {
      "type": "number",
      "minimum": -180,
      "maximum": 180
    },
    "Latitude": {
      "type": "number",
      "minimum": -90,
      "maximum": 90
    }
  },
  "required": ["_id", "Bssid", "Ssid", "Frequency", "Capabilities", "Longitude", "Latitude"],
  "additionalProperties": false
}
```

This application is part of a toolchain. For full context, please see:

🗄️ https://github.com/marianpekar/wifi-map-guide
