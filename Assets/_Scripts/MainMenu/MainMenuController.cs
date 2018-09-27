using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum MenuState {
  MAIN,
  SETUP,
  SETTINGS,
  CONFIRM,
  SETTINGSCONFIRM,
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
  private GameObject GameSettingConfirmation;


  [SerializeField]
  public MenuState myState = MenuState.MAIN;

  #region Monobehaviour Lifecycle methods

    // Use this for initialization
    void Start () {
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
      SetMenusActive();
    }

    private void SetMenusActive() {
      switch(myState) {
        case MenuState.MAIN:
          GameSetup.SetActive(false);
          GameSetupConfirmation.SetActive(false);
          GameSettings.SetActive(false);
          GameSettingConfirmation.SetActive(false);
          break;
        case MenuState.SETUP:
          GameSetup.SetActive(true);
          GameSetupConfirmation.SetActive(false);
          GameSettings.SetActive(false);
          GameSettingConfirmation.SetActive(false);
          break;
        case MenuState.CONFIRM:
          GameSetup.SetActive(false);
          GameSetupConfirmation.SetActive(true);
          GameSettings.SetActive(false);
          GameSettingConfirmation.SetActive(false);
          break;
        case MenuState.SETTINGS:
          GameSetup.SetActive(false);
          GameSetupConfirmation.SetActive(false);
          GameSettings.SetActive(true);
          GameSettingConfirmation.SetActive(false);
          break;
        case MenuState.SETTINGSCONFIRM:
          GameSetup.SetActive(false);
          GameSetupConfirmation.SetActive(false);
          GameSettings.SetActive(false);
          GameSettingConfirmation.SetActive(true);
          break;
      }
    }
  #endregion

  #region MenuButton public methods

    public void SetupNewParty() {
      if (myState == MenuState.SETUP) myState = MenuState.MAIN;
      else myState = MenuState.SETUP;
    }

    public void StartNewParty() {
      myState = MenuState.CONFIRM;
    }

    public void AdjustSettings() {
      if (myState == MenuState.SETTINGS) myState = MenuState.MAIN;
      else myState = MenuState.SETTINGS;
    }

    public void ConfirmSettings() {
      myState = MenuState.SETTINGSCONFIRM;
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

  #endregion

  #region Input control methods
    public void OnUpdateSelector(string fieldName, int index) {
      gameOptions[fieldName] = index;
    }

  #endregion
}
