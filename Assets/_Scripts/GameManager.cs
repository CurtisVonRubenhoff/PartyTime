using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
  private List<Text> UI_PlayerStats = new List<Text>();
  public GameState currentState = GameState.PREGAME;
  [SerializeField]
  private GameObject playerPrefab;
  [SerializeField]
  private GameObject comPlayerPrefab;
  [SerializeField]
  private List<GameObject> dicePrefab = new List<GameObject>();
  [SerializeField]
  private Transform firstSpaceTransform;

  private int numHumans;
  private bool turnOrderDecided = false;
  public List<PlayerRoll> playerRolls = new List<PlayerRoll>();
  private List<GameObject> miniGames = new List<GameObject>();
  private int currentGameTurn = 1;
  private int gameTurns = 0;

  private int currentPlayerTurn = 0;

	// Use this for initialization
	void Start () {
    if (GameManager.instance == null) GameManager.instance = this;
	}

  void Update() {
    switch(currentState) {
      case GameState.TURNORDER:
        if (playerRolls.Count == 4) {
          DetermineTurnOrder();
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

  public void StartWithOptions() {
    UI_StartButton.SetActive(false);
    PlayerPrefs.SetInt("gameTurns", 35);
    PlayerPrefs.SetInt("playerNumber", 1);
    SetupGame();
  }

  private void SetupGame() {
    gameTurns = PlayerPrefs.GetInt("gameTurns");
    MakePlayers();
  }

  private void MakePlayers() {
    numHumans = PlayerPrefs.GetInt("playerNumber", 0);
    for (var i = 0; i < 4; i++) {
      var isHuman = (i < numHumans);
      var prefab = (isHuman) ? playerPrefab : comPlayerPrefab;
      var playerGO = GameObject.Instantiate(prefab, firstSpaceTransform.position, firstSpaceTransform.rotation) as GameObject;
      var thisPlayer = playerGO.GetComponent<GamePlayer>();
      GameObject dice = GameObject.Instantiate(dicePrefab[i], UI_TurnOrder.transform) as GameObject;

      thisPlayer.playerId = i;
      thisPlayer.myDice = dice.GetComponent<DiceRoller>();
      currentPlayers.Add(thisPlayer);
    }

    currentState = GameState.TURNORDER;
    AllPlayersRoll();
  }

  private void DecideMiniGame() {
    //var game = miniGames[Random.Range(0, miniGames.Count)];
    currentGameTurn++;
    if (currentGameTurn == gameTurns) {
      EndGame();
    } else {
      StartPlayerTurn(currentPlayers[currentPlayerTurn]);
    }
  }

  private void DetermineTurnOrder() {
    StopPlayersRolling();
    UI_TurnOrder.SetActive(true);
    var newPlayerList = new List<GamePlayer>();

    IEnumerable<PlayerRoll> query = playerRolls.OrderBy(roll => roll.value);
    var i = 0;
    foreach (PlayerRoll roll in query) {
      var player = currentPlayers[roll.playerId];
      player.turnOrder = i;
      newPlayerList.Add(player);
      player.UI_Stats = UI_PlayerStats[i];
      player.playerCash = 10;
      i++;
      player.myState = PlayerState.IDLE;
    }

    currentPlayers = newPlayerList;
    playerRolls.Clear();
    StartPlayerTurn(currentPlayers[currentPlayerTurn]);
  }

  private void StartPlayerTurn(GamePlayer player) {
    MakePlayerRoll(player.playerId, true);
    currentState = GameState.PLAYERROLLING;
  }

  private void HandlePlayerTurn(GamePlayer player) {
    currentState = GameState.PLAYERMOVING;
    playerRolls.Clear();
    player.StartCoroutine(player.MovePiece());
  }

  public void AddCashToPlayer(int playerId, int num) {
    var player = GetPlayerById(playerId);
    player.playerCash += num;
    if (player.playerCash < 0) player.playerCash = 0;
    CalculateRanks();
  }

  public void AddEmblemsToPlayer(int playerId, int num) {
    var player = GetPlayerById(playerId);
    player.playerEmblems += num;
    CalculateRanks();
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
    for (var i = 0; i < 4; i++) {
      MakePlayerRoll(i, true);
    }
  }

  private void StopPlayersRolling() {
    for (var i = 0; i < 4; i++) {
      MakePlayerRoll(i, false);
    }
  }

  private void EndGame() {
    var results = CalculateWinner();
  }

  private void CalculateRanks() {
    var sorted = currentPlayers.OrderByDescending(x => x.playerEmblems).ThenByDescending(x => x.playerCash).ToList();
    GamePlayer lastPlayer = new GamePlayer();

    for (var i = 0; i < 4; i++) {
      var player = sorted[i];
      var shouldCheckLast = (i > 0);
      var value = isRankSame(lastPlayer, player) ? lastPlayer.myRank.rank : i + 1;

      PlayerRank rank;
      rank.playerId = player.playerId;
      rank.rank = value;
      player.myRank = rank;
      lastPlayer = player;
    }
  }

  private bool isRankSame(GamePlayer r1, GamePlayer r2) {
    var sameCash = (r1.playerCash == r2.playerCash);
    var sameEmblems = (r1.playerEmblems == r2.playerEmblems);

    return (sameCash && sameEmblems);
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
}
