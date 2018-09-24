using UnityEngine;
using System.Collections.Generic;

public enum SpotType {
  RED,
  BLUE,
  HAPPEN,
  ITEM,
  CHANCE,
  PRESIDENT,
  BATTLE,
  CASINO
}

public class MapSpot : MonoBehaviour {
  public SpotType myType;

  public GameManager GM;
  public List<MapSpot> nextSpots = new List<MapSpot>();

  public List<GameObject> currentPieces = new List<GameObject>();

  private void Start() {
    GM = GameManager.instance;
  }

  public void AffectPlayer(int playerId) {
    var value = (myType == SpotType.BLUE) ? 3 : -3;

    GM.AddCashToPlayer(playerId, value);
    Utils.RedistributeWealth(transform.position, ref currentPieces);
  }

  public void OnTriggerEnter2D(Collider2D col) {
    if (col.tag == "Player") {
      currentPieces.Add(col.gameObject);
      Utils.RedistributeWealth(transform.position, ref currentPieces);
    }
  }

  public void OnTriggerExit2D(Collider2D col) {
    if (col.tag == "Player") {
      currentPieces.Remove(col.gameObject);
      Utils.RedistributeWealth(transform.position, ref currentPieces);
    }
  }
}
