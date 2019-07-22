using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ComPlayerController : MonoBehaviour {

  [SerializeField]
  private GamePlayer myPlayer;
  private GameManager GM;

	// Use this for initialization
	void Start () {
		GM = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (myPlayer.myState == PlayerState.ROLLING) {
      StartCoroutine(WaitAndRoll());
    }
	}

  IEnumerator WaitAndRoll()
  {
    var seed = Random.Range(0, 5.5f);
    yield return new WaitForSeconds(seed);
    myPlayer.RollDice();
  }

  public IEnumerator WaitAndChooseDirection()
  {
    var seed = Random.Range(0.5f, 1.5f);
    var iShouldGo = PathFinder.WhichWayToTargetType(myPlayer.currentSpot, SpotType.EMBLEM);
    yield return new WaitForSeconds(seed);

    myPlayer.ChooseDirection(iShouldGo);
  }
}
