using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  [SerializeField]
  private GamePlayer myPlayer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (myPlayer.myState == PlayerState.ROLLING) {
      if (Input.GetKeyDown(KeyCode.Space)) {
        myPlayer.RollDice();
      }
    }
	}
}
