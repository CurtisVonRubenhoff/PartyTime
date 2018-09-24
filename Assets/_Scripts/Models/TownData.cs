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
  
  public void Start() {
    GM = GameManager.instance;
    GM.SetNewTown(transform.GetChild(0));
  }

  public void Update() {
    // beep boop
  }
}
