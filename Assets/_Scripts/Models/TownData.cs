using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*
  TownData must be places on the parent of a PartyTown prefab
  The prefab's FIRST child NEEDS to be an empty GameObject at {0, 0, 0}
  parented to the MapSpots

  This way, we can easily access them from the GameManager if we need to.
*/

public class TownData: MonoBehaviour {
  private GameManager GM;

  public List<GameObject> SpotObjectList = new List<GameObject>();
  public List<MapSpot> SpotList = new List<MapSpot>();

  
  public void Start() {
    GM = GameManager.instance;

    foreach (var obj in SpotObjectList) {
      SpotList.Add(obj.GetComponent<MapSpot>());
    }

    GM.SetNewTown(this);
  }

  public void Update() {
    // beep boop
  }
}
