using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class Utils {

  

  public static void CalculateRanks(List<GamePlayer> players) {
    var sorted = players
      .OrderByDescending(x => x.playerEmblems)
      .ThenByDescending(x => x.playerCash)
      .ToList();
    GamePlayer lastPlayer = new GamePlayer();

    for (var i = 0; i < Constants.MAX_PLAYERS; i++) {
      var thisPlayer = sorted[i];

      Utils.AssignRank(ref thisPlayer, ref lastPlayer, i);
    }
  }

  public static void AssignRank(
    ref GamePlayer player,
    ref GamePlayer lastPlayer,
    int index
  ) {
    var value = Utils.isRankSame(lastPlayer, player) ?
      lastPlayer.myRank.rank :
      index + 1;
    PlayerRank rank = new PlayerRank(){
      playerId = player.playerId,
      rank = value
    };

    player.myRank = rank;
    lastPlayer = player;
  }

  public static bool isRankSame(GamePlayer r1, GamePlayer r2) {
    var sameCash = (r1.playerCash == r2.playerCash);
    var sameEmblems = (r1.playerEmblems == r2.playerEmblems);

    return (sameCash && sameEmblems);
  }

  public static void MakePlayers(
    ref List<GamePlayer> players,
    GameObject pPrefab,
    GameObject cPrefab,
    List<GameObject> dicePrefab,
    Transform first,
    Transform diceParent
  ) {
    for (var i = 0; i < Constants.MAX_PLAYERS; i++) {
      var numHumans = PlayerPrefs.GetInt("PlayerCount", 0) + 1;
      var isCPU = !(i < numHumans);
      var prefab = (isCPU) ? cPrefab : pPrefab;
      var thisPlayer = Utils.MakePlayer(
        i,
        prefab,
        dicePrefab[i],
        first,
        diceParent,
        isCPU
      );

      players.Add(thisPlayer);
    }
  }

  public static GamePlayer MakePlayer(
    int playerId,
    GameObject prefab,
    GameObject dicePrefab,
    Transform first,
    Transform diceParent,
    bool isCPU
  ) {
    
    var playerGO = GameObject.Instantiate(prefab, first.position, first.rotation) as GameObject;
    var thisPlayer = playerGO.GetComponent<GamePlayer>();
    GameObject dice = GameObject.Instantiate(dicePrefab, diceParent) as GameObject;

    thisPlayer.playerId = playerId;
    thisPlayer.myDice = dice.GetComponent<DiceRoller>();
    thisPlayer.myRank = new PlayerRank() {
      playerId = playerId,
      rank = 1
    };
    thisPlayer.isCPU = isCPU;
    
    if(isCPU){
      thisPlayer.myCom = playerGO.GetComponent<ComPlayerController>();
    }

    return thisPlayer;
  }

  /*
    void DetermineTurnOrder(
      ref List<PlayerRoll> playerRolls,
      ref List<GamePlayer> currentPlayers,
      ref List<Text> playerStats,
      ref List<Text> playerRanks
    )

    Accepts List refs for playerRolls, currentPlayers, playerStats, and playerRanks.
      The method sorts the GamePlayers's rolls and updates references to UI elements
      the GamePlayer uses throughout the game.
  */
  public static void DetermineTurnOrder(
    ref List<PlayerRoll> playerRolls,
    ref List<GamePlayer> currentPlayers,
    ref List<Text> playerStats,
    ref List<Text> playerRanks
  ) {
    var newPlayerList = new List<GamePlayer>();
    IEnumerable<PlayerRoll> query = playerRolls.OrderByDescending(roll => roll.value);
    var i = 0;

    foreach (PlayerRoll roll in query) {
      var player = currentPlayers[roll.playerId];

      player.turnOrder = i;
      player.UI_Stats = playerStats[i];
      player.UI_Rank = playerRanks[i];
      player.playerCash = Constants.PLAYER_START_CASH;
      player.myState = PlayerState.IDLE;
      newPlayerList.Add(player);
      i++;
    }

    currentPlayers = newPlayerList;
  }


  /*
    Vector3 CalculatePosition(
      Vector3 centerPoint,
      int numOfPieces,
      int pieceIndex
    )

    Returns Vector3 representing position that GamePlayer should move to. Uses some
      basic Trignometry to evenly distribute multiple GamePlayers about the center
      of the MapSpot's position. 
  */
  public static Vector3 CalculatePosition(
    Vector3 centerPoint,
    int numOfPieces,
    int pieceIndex
  ) {

    // If there's only one GamePlayer on the MapSpot, we can quit out early and just
    //  tell the piece to use the centerPoint's position with the y offset added.
    if (numOfPieces == 1) {
      return new Vector3(
        centerPoint.x,
        centerPoint.y + Constants.PIECE_Y_OFFSET,
        centerPoint.z
      );
    }

    // First we figure out the segmentAngle for the number of GamePlayers on this MapSpot
    //  For the sake of speed, we forgo a calculation and use a Dictionary defined in Constants
    //
    // The angles come out as follows:
    //
    //  numOfPieces | segmentAngle in radians | segmentAngle in degrees
    //  ---------------------------------------------------------------
    //  1           | 0                       | 0
    //  2           | Pi                      | 180
    //  3           | 2*Pi/3                  | 120
    //  4           | Pi/2                    | 90

    float segmentAngle = Constants.SEGMENT_ANGLE_LOOKUP[numOfPieces];

    // Next we multiply the segmentAngle by which number GamePlayer we're dealing with.
    //  Doing this lets us know the angle between vector2 {1, 0} to the vector2
    //  defined by the GamePlayer's new position P {P.x, P.y} needs to be to position
    //  it corrently.
    float thisSegmentAngle = segmentAngle * pieceIndex;
    float dist = Constants.PIECE_DISTANCE;

    // Here's the trig. Cos(θ) gives you the x component of the point 1 unit from
    //  the centerPoint Sin(θ) gives you the y component. We multiply both values
    //  by the constant PIECE_DISTANCE to scale the value appropriately.
    float val1 = Mathf.Cos(thisSegmentAngle) * dist;
    float val2 =  Mathf.Sin(thisSegmentAngle) * dist;

    // Add the calculated values to the x and z components of centerPoint to get the
    //  world space coordinates we need this GamePlayer to be in. Add the constant
    //  PIECE_Y_OFFSET to the y component so that the GamePlayer aren't inside the MapSpot.
    //
    // NOTE: While Sin(θ) gave us the y component, we use that value for z of the
    //  calculated position because we aren't positioning the GamePlayers vertically.
    float posX = val1 + centerPoint.x;
    float posY = centerPoint.y + Constants.PIECE_Y_OFFSET;
    float posZ = val2 + centerPoint.z;

    return new Vector3(posX, posY, posZ);
  }
}
