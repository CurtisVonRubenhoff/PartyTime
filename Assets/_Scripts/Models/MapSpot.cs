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

  public float pieceDistance = .1f;

  private void Start() {
    GM = GameManager.instance;
  }

  public void AffectPlayer(int playerId) {
    var value = (myType == SpotType.BLUE) ? 3 : -3;

    GM.AddCashToPlayer(playerId, value);
    RedistributeWealth();
  }

  private void RedistributeWealth() {
    var count = currentPieces.Count;

    if (count == 0) return;

    for (var i = 0; i < count; i++) {
      var piece = currentPieces[i];
      var position = CalculatePosition(count, i);      

      piece.gameObject.transform.position = position; 
    }
  }

  public Vector3 CalculatePosition(int count, int index) {
    if (count == 1) {
      return transform.position;;
    }

    var angle = (2f * Mathf.PI) / count;
    var thisAngle = angle * index;
    var val1 = Mathf.Cos(thisAngle) * pieceDistance;
    var val2 =  Mathf.Sin(thisAngle) * pieceDistance;
    var posX = val1 + gameObject.transform.position.x;
    var posY = val2 + gameObject.transform.position.y;

    return new Vector3(posX, posY, 0);
  }

  public void OnTriggerEnter2D(Collider2D col) {
    if (col.tag == "Player") {
      var player = col.gameObject.GetComponent<GamePlayer>();
      currentPieces.Add(col.gameObject);
      RedistributeWealth();
    }
  }

  public void OnTriggerExit2D(Collider2D col) {
    if (col.tag == "Player") {
      currentPieces.Remove(col.gameObject);
      RedistributeWealth();
    }
  }
}
