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
    //{1, "Panchromia"},
  };

  public static class GreatOneMessage {
    public static string EmblemSpotLanding = "CONTENDER! You have made it to the sacred, chosen Spot. Would you like to exchange your cash for a marvelous emblem?";
    public static string EmblemSpotPoor = "Someone get boorish oaf away from me! How dare present you yourself to me with so little financial worth. YOU MAKE ME SICK!!!";
    public static string EmblemSpotNo = "What a shame! I suspect you know how to win the crowd following your own path. SO BE IT!";
    public static string EmblemSpotYes = "Take it and rejoice! You're one step closer to victory";
  }
}
