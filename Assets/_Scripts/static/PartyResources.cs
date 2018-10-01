using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PartyResources {
  public static Dictionary<int, string> Towns = new Dictionary<int, string>() {
    {0, "Prefabs/Towns/Prototown"},
  };

  public static Dictionary<string, string> MapSpotMaterial = new Dictionary<string, string>() {
    {"Blue", "Materials/Map Spots/Blue Spots"},
    {"Red", "Materials/Map Spots/Red Spots"},
    {"Green", "Materials/Map Spots/Green Spots"},
    {"Emblem", "Materials/Map Spots/Emblem"},
  };

  public static Dictionary<SpotType, Color> MapSpotColor = new Dictionary<SpotType, Color>() {
    {SpotType.RED, new Color(1f ,0f, 0f, .5f)},
    {SpotType.BLUE, new Color(0f ,0f, 1f, .5f)},
    {SpotType.EMBLEM, new Color(0f ,0f, 1f, .5f)},
  };
}
