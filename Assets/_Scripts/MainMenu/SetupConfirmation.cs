using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupConfirmation : MonoBehaviour {
  [SerializeField]
  private Text confirmationText;
  [SerializeField]
  private MainMenuController mMenu;

  void OnEnable()
  {
    var playerNum = mMenu.gameOptions["PlayerCount"];
    var turnNum = mMenu.gameOptions["TurnCount"];
    var gameMap = mMenu.gameOptions["GameMap"];

    var playerLabel = (playerNum > 0) ? string.Format("{0} Players", playerNum + 1) : "1 Player";
    confirmationText.text = string.Format(
      "Are you sure you want to start the game with these settings?\n\n{0}\n{1} Turns\n{2}",
      playerLabel,
      TextLookup.TurnText[turnNum],
      TextLookup.MapNames[gameMap]
    );
  }
}
