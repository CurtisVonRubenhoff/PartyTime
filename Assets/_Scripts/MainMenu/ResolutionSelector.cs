using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSelector : OptionSelector {

  public void Start() {
    var resolutions = Screen.resolutions;

    foreach (var res in resolutions) {
      optionList.Add(res.ToString());
    }

    currentSelection = mMenu.gameOptions[FieldName];

    base.Start();
  }

  public void Update() {
    base.Update();
  }

  public void CycleLeft() {
    base.CycleLeft();
  }

  public void CycleRight() {
    base.CycleRight();
  }

  public void SetButtonsActive(bool isTrue) {
    base.SetButtonsActive(isTrue);
  }
}
