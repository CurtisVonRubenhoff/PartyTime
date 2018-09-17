using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState {
  IDLE,
  ROLLING,
  MOVING,
}

public struct PlayerRank {
  public int playerId;
  public int rank;
}

public class GamePlayer : MonoBehaviour {
  public int playerId = -1;
  public int playerCash = 0;
  public int playerEmblems = 0;
  public int turnOrder;
  public PlayerRank myRank;

  public PlayerState myState = PlayerState.IDLE;
  public DiceRoller myDice;
  public MapSpot currentSpot;
  public int movesLeft = 0;
  private GameManager GM;

  public Text UI_Stats;

  private void Awake() {
    GM = GameManager.instance;
  }

  private void Update() {
    var moving =  myState == PlayerState.MOVING;
    var shouldRoll = (GM.currentState == GameState.TURNORDER || myState == PlayerState.ROLLING || moving);
    var shouldUpdateUI = (GM.currentState != GameState.TURNORDER && GM.currentState != GameState.PREGAME);

    if (shouldRoll) {
      myDice.gameObject.SetActive(true);
    } else {
      myDice.gameObject.SetActive(false);
    }

    if (myState == PlayerState.IDLE) myDice.gameObject.SetActive(false);
    if (moving) myDice.currentValue = movesLeft;

    if(shouldUpdateUI){
      UI_Stats.text = string.Format("{3}\nPlayer: {0}\nCash: {1}\nEmblems: {2}", (playerId + 1), playerCash, playerEmblems, TextLookup.RankText[myRank.rank]);
    }
  }

  public void RollDice() {
    if (myDice.isRolling) {
      myState = (GM.currentState == GameState.TURNORDER) ? myState : PlayerState.MOVING;
      PlayerRoll roll = new PlayerRoll() {
        playerId = playerId,
        value = myDice.StopDice()
      };

      movesLeft = roll.value;
      GM.InputPlayerRoll(roll);
    }
  }

  public IEnumerator LandOnSpot() {
    if (currentSpot == null) yield break;
    myState = PlayerState.IDLE;
    currentSpot.AffectPlayer(playerId);

    yield return new WaitForSeconds(.5f);
    GM.FinishedMove();
  }

  private void OnTriggerEnter2D(Collider2D col) {
    if(col.tag == "MapSpot") {
      currentSpot = col.gameObject.GetComponent<MapSpot>();
    }
  }
}
