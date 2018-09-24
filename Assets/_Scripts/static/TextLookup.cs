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
}
