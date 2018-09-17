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

  public static IEnumerator MovePiece(GamePlayer player) {
    while (player.movesLeft > 0) {
      var nextSpot = player.currentSpot.nextSpots[0];
      var finalStop = player.movesLeft == 1;

      yield return player.StartCoroutine(Utils.MoveMe(player.transform,nextSpot, finalStop));
      yield return new WaitForSeconds(Constants.MOVE_DELAY);
      player.movesLeft--;
    }

    player.StartCoroutine(player.LandOnSpot());
  }

  public static IEnumerator MoveMe(Transform transform, MapSpot nextSpot, bool finalStop)
  {
    var t = 0f;
    var position = transform.position;
    var target = nextSpot.gameObject.transform.position;
    var moveTime = Constants.MOVE_TIME;

    if (finalStop) {
      var spotCount = nextSpot.currentPieces.Count;

      target = Utils.CalculatePosition(target, spotCount + 1,  spotCount);
    }
    var distance = Vector3.Distance(position, target);
    var startTime = Time.time;

    while(t < moveTime) {
      t += Time.deltaTime;
      var frac = t/moveTime;

      transform.position = Vector3.Lerp(position, target, frac);
      yield return null;
    }

    yield return null;
  }

  public static void AssignRank(ref GamePlayer player, ref GamePlayer lastPlayer, int index) {
    var shouldCheckLast = (index > 0);
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
      var numHumans = PlayerPrefs.GetInt("playerNumber", 0);
      var isHuman = (i < numHumans);
      var prefab = (isHuman) ? pPrefab : cPrefab;
      var thisPlayer = Utils.MakePlayer(
        i,
        prefab,
        dicePrefab[i],
        first,
        diceParent
      );

      players.Add(thisPlayer);
    }
  }

  public static GamePlayer MakePlayer(
    int playerId,
    GameObject prefab,
    GameObject dicePrefab,
    Transform first,
    Transform diceParent
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

    return thisPlayer;
  }

  public static void DetermineTurnOrder(
    ref List<PlayerRoll> rolls,
    ref List<GamePlayer> players,
    ref List<Text> pStats
  ) {
    var newPlayerList = new List<GamePlayer>();
    IEnumerable<PlayerRoll> query = rolls.OrderBy(roll => roll.value);
    var i = 0;

    foreach (PlayerRoll roll in query) {
      var player = players[roll.playerId];

      player.turnOrder = i;
      player.UI_Stats = pStats[i];
      player.playerCash = Constants.PLAYER_START_CASH;
      player.myState = PlayerState.IDLE;
      newPlayerList.Add(player);
      i++;
    }

    players = newPlayerList;
  }

  public static void RedistributeWealth(Vector3 pos, ref List<GameObject> currentPieces) {
    var count = currentPieces.Count;

    if (count == 0) return;

    for (var i = 0; i < count; i++) {
      var piece = currentPieces[i];
      var position = Utils.CalculatePosition(pos, count, i);      

      piece.transform.position = position; 
    }
  }

  public static Vector3 CalculatePosition(Vector3 pos, int count, int index) {
    if (count == 1) {
      return pos;
    }

    var angle = (2f * Mathf.PI) / count;
    var thisAngle = angle * index;
    var dist = Constants.PIECE_DISTANCE;
    var val1 = Mathf.Cos(thisAngle) * dist;
    var val2 =  Mathf.Sin(thisAngle) * dist;
    var posX = val1 + pos.x;
    var posY = val2 + pos.y;

    return new Vector3(posX, posY, 0);
  }
}