using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] private InputActionReference SettingsToggleControl;
    [SerializeField] private InputActionReference QuitControl;
    [SerializeField] private InputActionReference ContinueControlKey;
    [SerializeField] private InputActionReference ContinueControlController;

    [SerializeField] private InputActionReference NewGameControl;
    [SerializeField] private InputActionReference ToggleSchemeControl;
    [SerializeField] private InputActionReference CreditsControl;

    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private GameObject settingsUIKeyboard;
    [SerializeField] private GameObject settingsUIController;

    [SerializeField] private GameObject menuUIKeyboard;
    [SerializeField] private GameObject menuUIController;

    private GameState gameState;


    private bool inSettings;
    string sceneName;

    CharacterController player;
    private bool inCredits;

    private void Start()
    {
        gameState = Resources.Load<GameState>("GameStateObject");
        Time.timeScale = 1;

        inSettings = false;
        inCredits = false;

        // Create a temporary reference to the current scene.
        Scene currentScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        sceneName = currentScene.name;

        if (sceneName == "Menu")
        {
            QuitControl.action.Enable();
            SettingsToggleControl.action.Disable();
            ContinueControlKey.action.Enable();
            ContinueControlController.action.Enable();
            NewGameControl.action.Enable();
            CreditsControl.action.Enable();
        }
        else
        {
            QuitControl.action.Disable();
            SettingsToggleControl.action.Enable();
            ContinueControlKey.action.Disable();
            ContinueControlController.action.Disable();
            NewGameControl.action.Disable();
            CreditsControl.action.Disable();
        }
    }

    private void Update()
    {
        if (SettingsToggleControl.action.triggered && sceneName != "Menu")
        {
            if (!inSettings)
            {
                inSettings = true;
                ToggleSchemeControl.action.Enable();

                QuitControl.action.Enable();
                ContinueControlKey.action.Enable();
                ContinueControlController.action.Enable();
                NewGameControl.action.Enable();

                Time.timeScale = 0;
                if (gameState.useController)
                {
                    settingsUIController.SetActive(true);
                    settingsUIKeyboard.SetActive(false);
                }
                else
                {
                    settingsUIKeyboard.SetActive(true);
                    settingsUIController.SetActive(false);
                }
            }
            else
            {
                inSettings = false;
                ToggleSchemeControl.action.Disable();

                Time.timeScale = 1;

                settingsUIKeyboard.SetActive(false);
                settingsUIController.SetActive(false);

                QuitControl.action.Disable();
                ContinueControlKey.action.Disable();
                ContinueControlController.action.Disable();
                NewGameControl.action.Disable();
            }
        }

        if (ToggleSchemeControl.action.triggered && inSettings)
        {
            gameState.ToggleControlScheme();

            if (gameState.useController)
            {
                settingsUIController.SetActive(true);
                settingsUIKeyboard.SetActive(false);
            }
            else
            {
                settingsUIKeyboard.SetActive(true);
                settingsUIController.SetActive(false);
            }
        }

        if (QuitControl.action.triggered)
        {
            if (inSettings && sceneName != "Menu")
            {
                SceneManager.LoadScene("Menu");
            }
            else if (sceneName == "Menu")
            {
                Application.Quit();
            }
        }

        if(sceneName == "Menu")
        {
            // play the game with keyboard
            if (ContinueControlKey.action.triggered)
            {
                SceneManager.LoadScene("Alphafest_Level");
                gameState.useController = false;
            }

            //play the game with controller
            if (ContinueControlController.action.triggered)
            {
                SceneManager.LoadScene("Alphafest_Level");
                gameState.useController = true;
            }

            if (CreditsControl.action.triggered)
            {
                inCredits = !inCredits;
                creditsPanel.SetActive(inCredits);
            }
        }
    }
}
