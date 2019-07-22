using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState {
  IDLE,
  ROLLING,
  CHOOSE_DIRECTION,
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
  public ComPlayerController myCom;
  public bool isCPU;

  public Text UI_Stats;
  public Text UI_Rank;
  private DirectionSelector ds;

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

    handlePlayerStateActions();
    if (moving) myDice.currentValue = movesLeft;

    if(shouldUpdateUI){
      UI_Stats.text = string.Format(
        "Player: {0}\nCash: {1}\nEmblems: {2}",
        (playerId + 1),
        playerCash,
        playerEmblems        
      );
      UI_Rank.text = TextLookup.RankText[myRank.rank];
    }
  }

  private void handlePlayerStateActions() {
    switch(myState) {
      case PlayerState.IDLE:
        myDice.gameObject.SetActive(false);
        break;
      case PlayerState.CHOOSE_DIRECTION:
        SubscribeDirectionEventHandlers();
        break;
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

  public void ChooseDirection(MovementDirection direction) {
    GM.playerWantsToGo = direction;

    UnSubscribeDirectionEventHandlers();
    myState = PlayerState.MOVING;
  }

  public IEnumerator LandOnSpot() {
    if (currentSpot == null) yield break;

    myState = PlayerState.IDLE;
    currentSpot.AffectPlayer(playerId);
    yield return new WaitForSeconds(Constants.MOVE_DELAY);

    GM.FinishedMove();
  }

  private void OnTriggerEnter(Collider col) {
    if(col.tag == "MapSpot") {
      currentSpot = col.gameObject.GetComponent<MapSpot>();
    }
  }

  public void SubscribeDirectionEventHandlers() {
    ds = DirectionSelector.instance;

    ds.UpButton.onClick.AddListener(delegate{ChooseDirection(MovementDirection.UP);});
    ds.DownButton.onClick.AddListener(delegate{ChooseDirection(MovementDirection.DOWN);});
    ds.LeftButton.onClick.AddListener(delegate{ChooseDirection(MovementDirection.LEFT);});
    ds.RightButton.onClick.AddListener(delegate{ChooseDirection(MovementDirection.RIGHT);});
    ds.UpLeftButton.onClick.AddListener(delegate{ChooseDirection(MovementDirection.UPLEFT);});
    ds.UpRightButton.onClick.AddListener(delegate{ChooseDirection(MovementDirection.UPRIGHT);});
    ds.DownLeftButton.onClick.AddListener(delegate{ChooseDirection(MovementDirection.DOWNLEFT);});
    ds.DownRightButton.onClick.AddListener(delegate{ChooseDirection(MovementDirection.DOWNRIGHT);});
  }

  public void UnSubscribeDirectionEventHandlers() {
    ds = DirectionSelector.instance;

    ds.UpButton.onClick.RemoveAllListeners();
    ds.DownButton.onClick.RemoveAllListeners();
    ds.LeftButton.onClick.RemoveAllListeners();
    ds.RightButton.onClick.RemoveAllListeners();
    ds.UpLeftButton.onClick.RemoveAllListeners();
    ds.UpRightButton.onClick.RemoveAllListeners();
    ds.DownLeftButton.onClick.RemoveAllListeners();
    ds.DownRightButton.onClick.RemoveAllListeners();
  }
}
