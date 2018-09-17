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

  public List<GameObject> currentPieces = new List<GameObject>();

  public float pieceDistance = .1f;

  private void Start() {
    GM = GameManager.instance;
  }

  public void AffectPlayer(int playerId) {
    Debug.Log(string.Format("affecting player {0}", playerId));
    var value = (myType == SpotType.BLUE) ? 3 : -3;
    GM.AddCashToPlayer(playerId, value);
    RedistributeWealth();
  }

  private void RedistributeWealth() {
    var count = currentPieces.Count;
    Debug.Log(string.Format("realigning {0} players", count));

    if (count == 0) return;

    if (count == 1) {
      currentPieces[0].transform.position = transform.position;
      return;
    }

    var angle = (2f * Mathf.PI) / count;

    for (var i = 0; i < count; i++) {
      var thisAngle = angle * i;
      var piece = currentPieces[i];
      var val1 = Mathf.Cos(thisAngle) * pieceDistance;
      var val2 =  Mathf.Sin(thisAngle) * pieceDistance;
      var posX = val1 + gameObject.transform.position.x;
      var posY = val2 + gameObject.transform.position.y;

      piece.gameObject.transform.position = new Vector3(posX, posY, 0);
    }
  }

  public void OnTriggerEnter2D(Collider2D col) {
    if (col.tag == "Player") {
      var player = col.gameObject.GetComponent<GamePlayer>();

      currentPieces.Add(col.gameObject);
      player.stopMoving = true;
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
