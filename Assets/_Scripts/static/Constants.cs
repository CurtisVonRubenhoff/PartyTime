using System.Collections.Generic;

public static class Constants {
  public static float MOVE_TIME = 0.25f;
  public static float MOVE_DELAY = 0.016f;
  public static float PIECE_Y_OFFSET = 0.01f;
  public static float PIECE_DISTANCE = 0.4f;
  public static int MAX_PLAYERS = 4;
  public static int PLAYER_START_CASH = 10;
  public static Dictionary<int, float> SEGMENT_ANGLE_LOOKUP = new Dictionary<int, float>() {
    {0, 0.0f},
    {1, 0.0f},
    {2, 3.14159f},
    {3, 2.0944f},
    {4, 1.5708f},
    {5, 1.0472f},
    {6, 0.523599f},
    {7, 0.897597f},
    {8, 0.785398f}
  };
}
