using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSelector : OptionSelector {

  public override void Start() {
    base.Start();
    var resolutions = Screen.resolutions;

    foreach (var res in resolutions) {
      optionList.Add(res.ToString());
    }

    currentSelection = mMenu.gameOptions[FieldName];
  }

  public override void Update() {
    base.Update();
  }

  public override void CycleLeft() {
    base.CycleLeft();
  }

  public override void CycleRight() {
    base.CycleRight();
  }

  public override void SetButtonsActive(bool isTrue) {
    base.SetButtonsActive(isTrue);
  }
}
