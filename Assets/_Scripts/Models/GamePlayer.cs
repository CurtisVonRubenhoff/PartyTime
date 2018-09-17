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
  public bool isHuman;
  public int turnOrder;
  public PlayerRank myRank;

  public PlayerState myState = PlayerState.IDLE;
  public DiceRoller myDice;
  public MapSpot currentSpot;
  public int movesLeft = 0;
  private GameManager GM;

  public Text UI_Stats;


  public void MovePiece() {
    while (movesLeft > 0) {
      var nextSpot = currentSpot.nextSpots[0];
      gameObject.transform.position = nextSpot.gameObject.transform.position;
      currentSpot = nextSpot;
      movesLeft--;
    }

    myState = PlayerState.IDLE;
    LandOnSpot();
    GM.FinishedMove();
  }

  private void Awake() {
    GM = GameManager.instance;
  }

  private void Update() {
    var shouldRoll = (GM.currentState == GameState.TURNORDER || myState == PlayerState.ROLLING);
    if (shouldRoll) {
      myDice.gameObject.SetActive(true);
    } else {
      myDice.gameObject.SetActive(false);
    }

    if (myState == PlayerState.MOVING) {
      MovePiece();
    }
    if(GM.currentState == GameState.GAME){
      UI_Stats.text = string.Format("Player: {0}\nCash: {1}\nEmblems: {2}\nRank: {3}", (playerId + 1), playerCash, playerEmblems, myRank.rank);
    }
  }

  public void RollDice() {
    if (myDice.isRolling) {
      PlayerRoll roll;
      roll.playerId = playerId;
      roll.value = myDice.StopDice();
      movesLeft = roll.value;
      GM.InputPlayerRoll(roll);
      myState = (GM.currentState == GameState.TURNORDER) ? PlayerState.IDLE: PlayerState.MOVING;
    }
  }

  private void LandOnSpot() {
    currentSpot.AffectPlayer(playerId);
  }

  private void OnTriggerEnter2D(Collider2D col) {
    if(col.tag == "MapSpot") {
      currentSpot = col.gameObject.GetComponent<MapSpot>();
    }
  }
}
