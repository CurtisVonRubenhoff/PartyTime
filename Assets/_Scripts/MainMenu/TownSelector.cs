using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownSelector : OptionSelector {

  public override void Start() {
    base.Start();
    var towns = TextLookup.MapNames;

    foreach (var town in towns) {
      optionList.Add(town.Value);
    }

    currentSelection = 0;
  }

  public override void SetButtonsActive(bool isTrue) {
    // nop
  }
}
