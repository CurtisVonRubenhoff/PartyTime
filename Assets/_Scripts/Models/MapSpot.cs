using UnityEngine;
using System.Collections.Generic;

public enum SpotType {
  RED,
  BLUE,
}

public class MapSpot : MonoBehaviour {
  public SpotType myType;

  public GameManager GM;
  public List<MapSpot> nextSpots = new List<MapSpot>();

  private void Start() {
    GM = GameManager.instance;
  }

  public void AffectPlayer(int playerId) {
    Debug.Log(string.Format("affecting player {0}", playerId));
    var value = (myType == SpotType.BLUE) ? 3 : -3;
    GM.AddCashToPlayer(playerId, value);
  }
}
