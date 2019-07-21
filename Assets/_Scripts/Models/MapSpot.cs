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
  CASINO,
  EMBLEM
}

public enum MovementDirection {
  UP,
  DOWN,
  LEFT,
  RIGHT,
  UPLEFT,
  UPRIGHT,
  DOWNLEFT,
  DOWNRIGHT,
  WAITING
}

public class MapSpot : MonoBehaviour {
  public SpotType myType;
  public int spotID;

  public GameManager GM;

  public List<MovementOptions> serializedMovementList = new List<MovementOptions>();
  public Dictionary<MovementDirection, MapSpot> nextSpots = new Dictionary<MovementDirection, MapSpot>();
  public Dictionary<MapSpot, MovementDirection> nextSpotsReverseLookup = new Dictionary<MapSpot, MovementDirection>();

  public List<GameObject> currentPieces = new List<GameObject>();

  private void Start() {
    GM = GameManager.instance;

    foreach (var thing in serializedMovementList) {
      nextSpots.Add(thing.myDirection, thing.mySpot);
      nextSpotsReverseLookup.Add(thing.mySpot, thing.myDirection);
    }
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
