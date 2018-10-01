using UnityEngine;
using System.Collections.Generic;

public enum SpotType {
  RED,
  BLUE,
  EMBLEM,
  HAPPEN,
  ITEM,
  CHANCE,
  PRESIDENT,
  BATTLE,
  CASINO
}

public class MapSpot : MonoBehaviour {
  public SpotType myType;
  private SpotType startType;

  public GameManager GM;
  public List<MapSpot> nextSpots = new List<MapSpot>();

  [SerializeField]
  private MeshRenderer renderer;

  public List<GameObject> currentPieces = new List<GameObject>();

  private void OnEnable() {
    GM = GameManager.instance;
    renderer = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
    startType = myType;
  }

  public void AffectPlayer(int playerId) {
    var value = 0;
    if (myType == SpotType.BLUE) value = 3;
    if (myType == SpotType.RED) value = -3;

    GM.SetPlayerColor(playerId, PartyResources.MapSpotColor[myType]);
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

  public void YouAreEmblem() {
    myType = SpotType.EMBLEM;
    renderer.material = Resources.Load<Material>(PartyResources.MapSpotMaterial["Emblem"]);
  }

  public void YouAreNotEmblem() {
    myType = SpotType.EMBLEM;
    var resource = "";

    switch (startType) {        
      case SpotType.RED:
        resource =  PartyResources.MapSpotMaterial["Red"];
        break;
      default:
        resource = PartyResources.MapSpotMaterial["Blue"];
        break;
    }

    myType = startType;
    renderer.material = Resources.Load<Material>(resource);
  }
}
