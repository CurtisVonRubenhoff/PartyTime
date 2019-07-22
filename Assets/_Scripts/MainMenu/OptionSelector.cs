using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour {
  public List<string> optionList = new List<string>();
  [SerializeField]
  public string FieldName;
  [SerializeField]
  public MainMenuController mMenu;
  [SerializeField]
  private Text myText;
  public int currentSelection = 0;

  [SerializeField]
  private MenuState activeState;
  [SerializeField]
  private Button leftButton;
  [SerializeField]
  private Button rightButton;


  public virtual void Start() {
    leftButton.onClick.AddListener(CycleLeft);
    rightButton.onClick.AddListener(CycleRight);
    currentSelection = mMenu.gameOptions[FieldName];
  }

  public virtual void Update() {
    myText.text = optionList[currentSelection];

    SetButtonsActive(activeState == mMenu.myState);
  }

  public virtual void CycleLeft() {
    currentSelection--;
    if (currentSelection < 0) currentSelection = optionList.Count - 1;
    mMenu.OnUpdateSelector(FieldName, currentSelection);
  }

  public virtual void CycleRight() {
    currentSelection++;
    if (currentSelection >= optionList.Count) currentSelection = 0;
    mMenu.OnUpdateSelector(FieldName, currentSelection);
  }

  public virtual void SetButtonsActive(bool isTrue) {
    leftButton.interactable = isTrue;
    rightButton.interactable = isTrue;
  }
}
