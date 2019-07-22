using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class GamePlayerMover {
  public static IEnumerator MoveGamePlayer(GamePlayer player) {
    GameManager gm = GameManager.instance;

    while (player.movesLeft > 0) {
      MapSpot nextSpot;
      var spots = player.currentSpot.nextSpots;
      nextSpot = spots[spots.Keys.ToList()[0]];

      if (spots.Count > 1) {
        gm.MakePlayerChooseDirection(player, spots.Keys.ToList());
        
        while (gm.playerWantsToGo == MovementDirection.WAITING) {
          yield return null;
        }

        var ds = DirectionSelector.instance;
        ds.DisableButtons(spots.Keys.ToList());
        nextSpot = spots[gm.playerWantsToGo];
      }

      var finalStop = player.movesLeft == 1;

      yield return player
        .StartCoroutine(
          GamePlayerMover.MoveMe(
            player,
            nextSpot,
            finalStop
          )
        );

      gm.playerWantsToGo = MovementDirection.WAITING;
      
      yield return new WaitForSeconds(Constants.MOVE_DELAY);
      player.movesLeft--;
    }

    player.StartCoroutine(player.LandOnSpot());
  }

  public static IEnumerator MoveMe(GamePlayer player, MapSpot nextSpot, bool finalStop)
  {
    var target = nextSpot.gameObject.transform.position;
    var transform = player.gameObject.transform;
    var spotCount = nextSpot.currentPieces.Count;

    target = Utils.CalculatePosition(target, spotCount + 1,  spotCount);
  
    yield return player.StartCoroutine(
      GamePlayerMover.MoveMe(
        transform,
        target,
        Constants.MOVE_TIME
      )
    );
  }

  public static IEnumerator MoveMe(Transform transform, Vector3 target, float moveTime)
  {
    var t = 0f;
    var position = transform.position;

    while(t < moveTime) {
      t += Time.deltaTime;
      var frac = t/moveTime;

      transform.position = Vector3.Lerp(position, target, frac);
      yield return null;
    }

    yield return null;
  }

  /*
    public static void RepositionGamePieces(
      Vector3 spotPosition,
      ref List<GameObject> currentPieces
    )

    Repositions game pieces found in ref currentPieces. This method gets called by
      a MapSpot when a piece enters its trigger.
  */
  public static void RepositionGamePieces(
    Vector3 spotPosition,
    ref List<GameObject> currentPieces
  ) {
    var count = currentPieces.Count;

    // Early exit for whern there aren't any GamePlayers on the MapSpot.
    if (count == 0) return;

    for (var i = 0; i < count; i++) {
      var piece = currentPieces[i];
      var player = piece.GetComponent<GamePlayer>();
      var piecePosition = Utils.CalculatePosition(spotPosition, count, i);

      // If the piece is currently in the MOVING state, then we don't need to bother
      //  trying to move it. It's on its own mission.
      if (player.myState == PlayerState.MOVING) continue;

      // Each GamePlayer needs to execute GamePlayerMover.MoveMe on its so to
      //  prevent weirdness.
      player.StartCoroutine(GamePlayerMover.MoveMe(piece.transform, piecePosition, Constants.MOVE_TIME/2f)); 
    }
  }
}
