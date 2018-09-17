using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour {

  public int currentValue = 10;
  [SerializeField]
  private Text DiceText;

  public bool isRolling = true;

  void Start() {
    currentValue = Random.Range(0, 10);
  }
	
	// Update is called once per frame
	void Update () {
    if (isRolling) {
      currentValue++;
      if (currentValue > 10) currentValue = 1;
    }

    DiceText.text = currentValue.ToString();
	}

  public int StopDice() {
    isRolling = false;
    return currentValue;
  }
}
