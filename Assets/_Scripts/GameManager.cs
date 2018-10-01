using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour {

  public static GameManager instance;
  [SerializeField]
  private List<GamePlayer> currentPlayers = new List<GamePlayer>();
  [SerializeField]
  private GameObject UI_TurnOrder;
  [SerializeField]
  private GameObject UI_StartButton;
  [SerializeField]
  private Text UI_TurnIndicator;
  [SerializeField]
  private List<PlayerUI> UI_PlayerStats = new List<PlayerUI>();
  public GameState currentState = GameState.PREGAME;
  [SerializeField]
  private GameObject playerPrefab;
  [SerializeField]
  private GameObject comPlayerPrefab;
  [SerializeField]
  private List<GameObject> dicePrefab = new List<GameObject>();
  private TownData currentTown;
  private Transform firstSpaceTransform;

  [SerializeField]
  private TextBoxUI UI_TextBox;
  [SerializeField]
  private GameObject UI_EmblemDialog;

  public List<PlayerRoll> playerRolls = new List<PlayerRoll>();
  private List<GameObject> miniGames = new List<GameObject>();
  private GameObject currentBoard;
  private int currentGameTurn = 1;
  private int gameTurns = 0;

  private int currentPlayerTurn = 0;

	// Use this for initialization
	void Start () {
    if (GameManager.instance == null) GameManager.instance = this;
    SetupBoard();
	}

  void Update() {
    switch(currentState) {
      case GameState.TURNORDER:
        if (playerRolls.Count == Constants.MAX_PLAYERS) {
          HandleFinishRolling();
        }
        break;
      case GameState.PLAYERROLLING:
        var player = currentPlayers[currentPlayerTurn];

        if (GetPlayerRoll(player.playerId) > 0) {
          HandlePlayerTurn(player);
        }
        break;
    }

    UI_TurnIndicator.text = string.Format("Current Turn:{0}", currentGameTurn);
  }

  private void SetupBoard() {
    currentBoard = GameObject.Instantiate(
      Resources.Load(
        PartyResources.Towns[
          PlayerPrefs.GetInt("GameMap")
        ]
      )
    ) as GameObject;
  }

  private void FinishGameSetup(){
    gameTurns = TextLookup.TurnText[PlayerPrefs.GetInt("TurnCount")];
    DecideNewEmblemLocation();

    Utils.MakePlayers(
      ref currentPlayers,
      playerPrefab,
      comPlayerPrefab,
      dicePrefab,
      firstSpaceTransform,
      UI_TurnOrder.transform
    );

    currentState = GameState.TURNORDER;
    AllPlayersRoll();
  }

  private void DecideNewEmblemLocation() {
    var randomIndex = Random.Range(0, currentTown.SpotList.Count);

    currentTown.SpotList[randomIndex].YouAreEmblem();
  }
  private void DecideMiniGame() {
    currentState = GameState.MINIGAME;
    // Do stuff here to start mini game.
    
    FinishMiniGame();
  }

  private void HandleFinishRolling() {
    StopPlayersRolling();
    UI_TurnOrder.SetActive(true);
    Utils.DetermineTurnOrder(ref playerRolls, ref currentPlayers, ref UI_PlayerStats);

    StartPlayerTurn(currentPlayers[currentPlayerTurn]);
  }

  private void FinishMiniGame() {
    currentGameTurn++;
    ResetPlayerColors();
    if (currentGameTurn == gameTurns) {
      EndGame();
    } else {
      StartPlayerTurn(currentPlayers[currentPlayerTurn]);
    }
  }

  private void ResetPlayerColors() {
    for (var i = 0; i < 4; i++) {
      SetPlayerColor(i, new Color(0f, 0f, 0f, 0.5f));
    }
  }

  private void StartPlayerTurn(GamePlayer player) {
    MakePlayerRoll(player.playerId, true);
    currentState = GameState.PLAYERROLLING;
  }

  private void HandlePlayerTurn(GamePlayer player) {
    currentState = GameState.PLAYERMOVING;
    playerRolls.Clear();
    player.StartCoroutine(Utils.MovePiece(player));
  }

  public void AddCashToPlayer(int playerId, int num) {
    var player = GetPlayerById(playerId);

    player.playerCash += num;
    if (player.playerCash < 0) player.playerCash = 0;
    Utils.CalculateRanks(currentPlayers);
  }

  public void AddEmblemsToPlayer(int playerId, int num) {
    var player = GetPlayerById(playerId);

    player.playerEmblems += num;
    if (num > 0) {
      AddCashToPlayer(playerId, num * Constants.EMBLEM_COST * -1);
    }
    Utils.CalculateRanks(currentPlayers);
  }

  public void InputPlayerRoll(PlayerRoll roll) {
    playerRolls.Add(roll);
  }

  public void FinishedMove() {
    playerRolls.Clear();
    currentPlayerTurn++;
    if (currentPlayerTurn < 4) {
      StartPlayerTurn(currentPlayers[currentPlayerTurn]);
    } else {
      currentPlayerTurn = 0;
      DecideMiniGame();
    }
  }

  private void MakePlayerRoll(int id, bool value) {
    var player = GetPlayerById(id);

    player.myDice.isRolling = value;
    player.myState = (value) ? PlayerState.ROLLING : PlayerState.IDLE;
  }

  private void AllPlayersRoll() {
    for (var i = 0; i < Constants.MAX_PLAYERS; i++) {
      MakePlayerRoll(i, true);
    }
  }

  private void StopPlayersRolling() {
    for (var i = 0; i < Constants.MAX_PLAYERS; i++) {
      MakePlayerRoll(i, false);
    }
  }

  private void EndGame() {
    SceneManager.LoadScene("MainMenu");
  }

  private GamePlayer CalculateWinner() {
    foreach(GamePlayer player in currentPlayers) {
      if (player.myRank.rank == 1) {
        return player;
      }
    }

    return null;
  }

  private GamePlayer GetPlayerById(int playerId) {
    foreach(GamePlayer player in currentPlayers) {
      if (player.playerId == playerId) return player;
    }

    return new GamePlayer();
  }
	
      
	private int GetPlayerRoll(int playerId) {
    foreach (PlayerRoll roll in playerRolls) {
      if (roll.playerId == playerId) return roll.value;
    }

    return -1;
  }

  public void SetNewTown(TownData thisTown) {
    currentTown = thisTown;
    firstSpaceTransform = currentTown.SpotObjectList[0].transform;
    FinishGameSetup();
  }

  public void StartEmblemEvent() {
    UI_TextBox.SpeakerName.text = "Great One";
    var player = currentPlayers[currentPlayerTurn];

    var playerCanBuy = player.playerCash >= Constants.EMBLEM_COST;
    if (playerCanBuy) {
      UI_TextBox.Message.text = TextLookup.GreatOneMessage.EmblemSpotLanding;
      UI_EmblemDialog.SetActive(true);
      player.myState = PlayerState.BUYING;
    } else {
      StartCoroutine(StartEmblemEventPoor());
    }

    player.myState = PlayerState.BUYING;

    UI_TextBox.gameObject.SetActive(true);
  }

  public void EmblemEventYes() {
      StartCoroutine(StartEmblemEventYes());
  }

  public void EmblemEventNo() {
    StartCoroutine(StartEmblemEventNo());
  }

  public IEnumerator StartEmblemEventPoor() {
    UI_TextBox.Message.text = TextLookup.GreatOneMessage.EmblemSpotPoor;

    yield return new WaitForSeconds(Constants.DIALOG_DELAY);
    FinishEmblemEvent();
  }
  public IEnumerator StartEmblemEventYes() {
    UI_EmblemDialog.SetActive(false);
    var player = currentPlayers[currentPlayerTurn];

    AddEmblemsToPlayer(player.playerId, 1);
    UI_TextBox.Message.text = TextLookup.GreatOneMessage.EmblemSpotYes;
    yield return new WaitForSeconds(Constants.DIALOG_DELAY);
    player.currentSpot.YouAreNotEmblem();
    DecideNewEmblemLocation();
    FinishEmblemEvent();
  }
  public IEnumerator StartEmblemEventNo() {
    UI_EmblemDialog.SetActive(false);
    UI_TextBox.Message.text = TextLookup.GreatOneMessage.EmblemSpotNo;
    yield return new WaitForSeconds(Constants.DIALOG_DELAY);
    FinishEmblemEvent();
  }

  private void FinishEmblemEvent() {
    currentPlayers[currentPlayerTurn].FinishEmblemStop();
    UI_TextBox.gameObject.SetActive(false);
    UI_EmblemDialog.SetActive(false);
  }

  public void SetPlayerColor(int pId, Color newColor) {
    var player = GetPlayerById(pId);
    player.UI_Stats.PlayerColor.color = newColor;
  }
}
