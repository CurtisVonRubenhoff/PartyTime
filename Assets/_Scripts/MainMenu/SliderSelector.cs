using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSelector : MonoBehaviour {
  [SerializeField]
  private string FieldName;
  [SerializeField]
  MainMenuController mMenu;
  [SerializeField]
  private Text myText;

  [SerializeField]
  private Slider mySlide;


  public void Start() {
    mySlide.onValueChanged.AddListener(delegate {this.onValueChanged();});
  }

  public void Update() {
    myText.text = string.Format("{0}",mySlide.value);
  }

  public void onValueChanged() {
    mMenu.OnUpdateSelector(FieldName, (int)mySlide.value);
  }
}
