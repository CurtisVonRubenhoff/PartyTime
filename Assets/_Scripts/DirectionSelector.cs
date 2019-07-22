using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DirectionSelector: MonoBehaviour {

  public static DirectionSelector instance;
  public GameObject SelectorParent;

  public Button UpButton;
  public Button DownButton;
  public Button LeftButton;
  public Button RightButton;
  public Button UpLeftButton;
  public Button UpRightButton;
  public Button DownLeftButton;
  public Button DownRightButton;
  

  public void Start() {
    if (instance == null) {
      instance = this;
    }
  }

  public void EnableButtons (List<MovementDirection> directions) {
    foreach(MovementDirection direction in directions) {
      EnableButton(direction);
    }

    SelectorParent.SetActive(true);
  }

  public void DisableButtons (List<MovementDirection> directions) {
    foreach(MovementDirection direction in directions) {
      DisableButton(direction);
    }

    SelectorParent.SetActive(false);
  }

  private void EnableButton(MovementDirection direction) {
    switch(direction) {
      case MovementDirection.UP:
        UpButton.gameObject.SetActive(true);
        break;
      case MovementDirection.DOWN:
        DownButton.gameObject.SetActive(true);
        break;
      case MovementDirection.LEFT:
        LeftButton.gameObject.SetActive(true);
        break;
      case MovementDirection.RIGHT:
        RightButton.gameObject.SetActive(true);
        break;
      case MovementDirection.UPLEFT:
        UpLeftButton.gameObject.SetActive(true);
        break;
      case MovementDirection.UPRIGHT:
        UpRightButton.gameObject.SetActive(true);
        break;
      case MovementDirection.DOWNLEFT:
        DownLeftButton.gameObject.SetActive(true);
        break;
      case MovementDirection.DOWNRIGHT:
        DownRightButton.gameObject.SetActive(true);
        break;
    }
  }

private void DisableButton(MovementDirection direction) {
    switch(direction) {
      case MovementDirection.UP:
        UpButton.gameObject.SetActive(false);
        break;
      case MovementDirection.DOWN:
        DownButton.gameObject.SetActive(false);
        break;
      case MovementDirection.LEFT:
        LeftButton.gameObject.SetActive(false);
        break;
      case MovementDirection.RIGHT:
        RightButton.gameObject.SetActive(false);
        break;
      case MovementDirection.UPLEFT:
        UpLeftButton.gameObject.SetActive(false);
        break;
      case MovementDirection.UPRIGHT:
        UpRightButton.gameObject.SetActive(false);
        break;
      case MovementDirection.DOWNLEFT:
        DownLeftButton.gameObject.SetActive(false);
        break;
      case MovementDirection.DOWNRIGHT:
        DownRightButton.gameObject.SetActive(false);
        break;
    }
  }
}
