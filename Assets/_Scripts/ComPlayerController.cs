using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    if (myPlayer.myState == PlayerState.BUYING) {
      StartCoroutine(WaitAndBuy());
    }
	}

  IEnumerator WaitAndRoll()
  {
    var seed = Random.Range(0, 5.5f);
    yield return new WaitForSeconds(seed);
    myPlayer.RollDice();
  }

  IEnumerator WaitAndBuy()
  {
    myPlayer.myState = PlayerState.IDLE;
    var seed = Random.Range(0, 2f);
    yield return new WaitForSeconds(seed);
    GM.EmblemEventYes();    
  }
}
