using System.Collections.Generic;

public static class TextLookup {
  public static Dictionary<int, string> RankText = new Dictionary<int, string>()
  {
    {1, "1st"},
    {2, "2nd"},
    {3, "3rd"},
    {4, "4th"}
  };

  public static Dictionary<int, int> TurnText = new Dictionary<int, int>()
  {
    {0, 20},
    {1, 35},
    {2, 50},
    {3, 60}
  };

  public static Dictionary<int, string> MapNames = new Dictionary<int, string>()
  {
    {0, "Prototown"},
  };

  public static Dictionary<int, string> Resolutions = new Dictionary<int, string>()
  {
    {0, "1280x720"},
    {1, "1024x768"},
    {2, "1366x768"},
    {3, "1280x800"},
    {4, "1440x900"},
    {5, "1600x900"},
    {6, "1280x1024"},
    {7, "1680x1050"},
    {8, "1920x1080"},
    {9, "1920x1200"},
  };
}
