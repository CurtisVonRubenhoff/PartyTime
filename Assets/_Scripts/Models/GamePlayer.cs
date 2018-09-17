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
  public bool isHuman;
  public int turnOrder;
  public PlayerRank myRank;

  public PlayerState myState = PlayerState.IDLE;
  public DiceRoller myDice;
  public MapSpot currentSpot;
  public int movesLeft = 0;
  private GameManager GM;
  public float moveTime = 1f;
  public bool stopMoving = false;

  public Text UI_Stats;


  public IEnumerator MovePiece() {
    while (movesLeft > 0) {
      var nextSpot = currentSpot.nextSpots[0];

      yield return StartCoroutine(MoveMe(nextSpot));
      yield return new WaitForSeconds(0.016f);
      movesLeft--;
    }

    StartCoroutine(LandOnSpot());
  }

  public IEnumerator MoveMe(MapSpot nextSpot)
  {
    var t = 0f;
    var position = transform.position;
    var target = nextSpot.gameObject.transform.position;

    if (movesLeft == 1) {
      var spotCount = nextSpot.currentPieces.Count;
      target = nextSpot.CalculatePosition(spotCount + 1,  spotCount);
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
      PlayerRoll roll;
      roll.playerId = playerId;
      roll.value = myDice.StopDice();
      movesLeft = roll.value;
      GM.InputPlayerRoll(roll);
    }
  }

  private IEnumerator LandOnSpot() {
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
