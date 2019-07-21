using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/*
  TownData must be places on the parent of a PartyTown prefab
  The prefab's FIRST child NEEDS to be an empty GameObject at {0, 0, 0}
  parented to the MapSpots

  This way, we can easily access them from the GameManager if we need to.
*/

public class TownData: MonoBehaviour {
  private GameManager GM;
  public Transform FirstSpotTransform;
  public MapSpot FirstSpot;
  public MapSpot StarSpot;

  public Dictionary<MapSpot, int> mapList = new Dictionary<MapSpot, int>();
  
  public void Start() {
    GM = GameManager.instance;
    GM.SetNewTown(this);
    PopulateMapList();
    ChooseSpotForStar();
  }

  private void PopulateMapList() {
    int current = 0;
    bool complete = false;
    Queue<MapSpot> toVisit = new Queue<MapSpot>();
    toVisit.Enqueue(FirstSpot);

    while (toVisit.Count > 0) {
      var thisSpot = toVisit.Dequeue();
      if (mapList.ContainsKey(thisSpot)) continue;

      thisSpot.spotID = current;
      mapList.Add(thisSpot, current);
      current++;
      
      var neighbors = thisSpot.nextSpots.Values.ToList();

      foreach (MapSpot spot in neighbors) {
        toVisit.Enqueue(spot);
      }
    }

    Debug.Log(string.Format("Found {0} MapSpots", mapList.Count));
  }

  public void ChooseSpotForStar() {
    var spots = mapList.Keys.ToList();
    var choice = (int)Mathf.Ceil(Random.Range(0, spots.Count));

    StarSpot = spots[choice];
    StarSpot.myType = SpotType.EMBLEM;
  }

  public MapSpot GetSpotById(int id) {
    var spots = mapList.Keys.ToList();
    var i = 0;

    while (i < spots.Count) {
      if (spots[i].spotID == id) {
        return spots[i];
      } else {
        i++;
      }
    }

    return FirstSpot;
  }

  public void Update() {
    // beep boop
  }
}
