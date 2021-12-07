using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] private InputActionReference SettingsToggleControl;
    [SerializeField] private InputActionReference QuitControl;
    [SerializeField] private InputActionReference ContinueControl;
    [SerializeField] private InputActionReference NewGameControl;
    [SerializeField] private InputActionReference ToggleSchemeControl;

    [SerializeField] private GameObject settingsUIKeyboard;
    [SerializeField] private GameObject settingsUIController;

    [SerializeField] private GameObject menuUIKeyboard;
    [SerializeField] private GameObject menuUIController;

    private bool inSettings;
    string sceneName;

    CharacterController player;

    private void Start()
    {
        inSettings = false;

        // Create a temporary reference to the current scene.
        Scene currentScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        sceneName = currentScene.name;

        if (sceneName == "Menu")
        {
            QuitControl.action.Enable();
            SettingsToggleControl.action.Disable();
            ContinueControl.action.Enable();
            NewGameControl.action.Enable();
        }
        else
        {
            QuitControl.action.Disable();
            SettingsToggleControl.action.Enable();
            ContinueControl.action.Disable();
            NewGameControl.action.Disable();
        }
    }

    private void Update()
    {
        if (SettingsToggleControl.action.triggered)
        {
            if (!inSettings)
            {
                Time.timeScale = 0;
                if (StoryManager.instance.useController)
                {
                    settingsUIController.SetActive(true);

                    QuitControl.action.Enable();
                    ContinueControl.action.Enable();
                    NewGameControl.action.Enable();
                }
                else
                {
                    menuUIController.SetActive(true);

                    QuitControl.action.Disable();
                    ContinueControl.action.Disable();
                    NewGameControl.action.Disable();
                }
            }
            else
            {
                Time.timeScale = 1;
                if (StoryManager.instance.useController)
                {
                    settingsUIController.SetActive(false);
                }
                else
                {
                    menuUIController.SetActive(false);
                }
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
        
        if (ContinueControl.action.triggered)
        {
            if (sceneName == "Menu")
            {
                SceneManager.LoadScene("Alphafest_Level");
            }
        }

        if (ToggleSchemeControl.action.triggered && inSettings)
        {
            StoryManager.instance.ToggleControlScheme();
        }
    }
}
