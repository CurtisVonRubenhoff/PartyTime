﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public enum MenuState {
  MAIN,
  SETUP,
  SETTINGS,
  DISPLAYSETTINGS,
  SOUNDSETTINGS,
  CONFIRM,
  SETTINGSCONFIRM,
}

public enum SubSettingsState {
  MAIN,
  SOUND,
  DISPLAY
}

public class MainMenuController : MonoBehaviour {

  [SerializeField]
  private List<Entry> inspectorEntries;
  public Dictionary<string, int> gameOptions = new Dictionary<string, int> ();

  [SerializeField]
  private GameObject GameSetup;

  [SerializeField]
  private GameObject GameSetupConfirmation;
  [SerializeField]
  private GameObject GameSettings;
  [SerializeField]
  private GameObject DisplaySettings;
  [SerializeField]
  private GameObject SoundSettings;
  [SerializeField]
  private GameObject GameSettingConfirmation;
  [SerializeField]
  private Text GameSettingConfirmationText;


  [SerializeField]
  public MenuState myState = MenuState.MAIN;
  [SerializeField]
  public SubSettingsState subSettingsState = SubSettingsState.MAIN;

  private Dictionary<MenuState, GameObject> MainStateToMenuObject = new Dictionary<MenuState, GameObject>();
  private Dictionary<SubSettingsState, GameObject> SubSettingsStateToMenuObject = new Dictionary<SubSettingsState, GameObject>();

	// Use this for initialization
	void Start () {
    MainStateToMenuObject.Add(MenuState.SETTINGS, GameSettings);
    MainStateToMenuObject.Add(MenuState.SETUP, GameSetup);
    MainStateToMenuObject.Add(MenuState.SETTINGSCONFIRM, GameSettingConfirmation);
    MainStateToMenuObject.Add(MenuState.CONFIRM, GameSetupConfirmation);

    SubSettingsStateToMenuObject.Add(SubSettingsState.DISPLAY, DisplaySettings);
    SubSettingsStateToMenuObject.Add(SubSettingsState.SOUND, SoundSettings);

		foreach (var entry in inspectorEntries) {
      gameOptions[entry.Key] = entry.Value;
    }

    // Gets current resolution and find its index in our list of acceptible ones.
    var currentRes = Screen.currentResolution;
    var resIndex = System.Array.IndexOf(Screen.resolutions, currentRes);

    gameOptions["ScreenResolution"] = resIndex;
    gameOptions["Fullscreen"] = Screen.fullScreen ? 0 : 1;
	}
	
	// Update is called once per frame
	void Update () {
		SetMainMenusActive();
    SetSubMenusActive();
	}

  private void SetMainMenusActive() {
    try {
      MainStateToMenuObject[myState].SetActive(true);
    } catch {}

    foreach (var state in MainStateToMenuObject.Keys.ToList()) {
        if (state != myState) {
          MainStateToMenuObject[state].SetActive(false);
        }
    }
  }

  private void SetSubMenusActive() {
    try {
      SubSettingsStateToMenuObject[subSettingsState].SetActive(true);
    } catch {}

    foreach (var state in SubSettingsStateToMenuObject.Keys.ToList()) {
        if (state != subSettingsState) {
          SubSettingsStateToMenuObject[state].SetActive(false);
        }
    }
  }


  public void SetupNewParty() {
    if (myState == MenuState.SETUP) myState = MenuState.MAIN;
    else myState = MenuState.SETUP;
  }

  public void StartNewParty() {
    myState = MenuState.CONFIRM;
  }

  public void AdjustSettings() {
    if (myState == MenuState.SETTINGS) {
      myState = MenuState.MAIN;      
    } else myState = MenuState.SETTINGS;
  }

  public void ShowDisplaySettings() {
    if (subSettingsState == SubSettingsState.DISPLAY) {
      subSettingsState = SubSettingsState.MAIN;
    } else {
      subSettingsState = SubSettingsState.DISPLAY;
    }
  }

  public void ShowSoundSettings() {
    if (subSettingsState == SubSettingsState.SOUND) subSettingsState = SubSettingsState.MAIN;
    else subSettingsState = SubSettingsState.SOUND;
  }

  public void ConfirmSettings() {
    if(DidSettingsChange()) {    
      myState = MenuState.SETTINGSCONFIRM;

      var res = Screen.resolutions[gameOptions["ScreenResolution"]];
      var fullScreen = (gameOptions["Fullscreen"] == 0) ? "Yes" : "No";


      GameSettingConfirmationText.text = $"Do you want to apply these settings?\n\nResolution:\n{res.width}x{res.height}@{res.refreshRate}\n\nFullscreen:\n{fullScreen}";
    } else {
      myState = MenuState.MAIN;
    }
  }

  public void ApplyNewSettings() {
    var res = Screen.resolutions[gameOptions["ScreenResolution"]];
    var fullScreen = gameOptions["Fullscreen"] == 0;

    Debug.Log(string.Format("Resolution changed to {0}x{1}@{2} FS:{3}", res.width, res.height, res.refreshRate, fullScreen));

    Screen.SetResolution(res.width, res.height, fullScreen, res.refreshRate);
    MainMenu();
  }

  public void ConfirmPartyStart() {
    foreach (var option in gameOptions) {
      PlayerPrefs.SetInt(option.Key, option.Value);
    }
    
    SceneManager.LoadScene("GameBoard");
  }

  public void MainMenu() {
    myState = MenuState.MAIN;
  }

  public void QuitGame() {
    Application.Quit();
  }

  public void OnUpdateSelector(string fieldName, int index) {
    gameOptions[fieldName] = index;
  }

  private bool DidSettingsChange() {
    var selectedRes = Screen.resolutions[gameOptions["ScreenResolution"]];
    var cuurentRes = Screen.currentResolution;

    var didResHStay = (selectedRes.height == cuurentRes.height);
    var didResWStay = (selectedRes.width == cuurentRes.width);
    var didResRefStay = (selectedRes.refreshRate == cuurentRes.refreshRate);

    bool didResolutionStay = didResHStay && didResWStay && didResRefStay;
    bool didFullScreenStay = ((gameOptions["Fullscreen"] == 0) == Screen.fullScreen);

    return !(didResolutionStay && didFullScreenStay);
  }
}
