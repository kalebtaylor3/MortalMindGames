using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public enum OPTIONS_TYPE { NONE = -1, AUDIO = 0, VIDEO = 1, CONTROLS = 2 }

public class OptionsMenu : MonoBehaviour
{
    
    [SerializeField] GameObject audioBtn, videoBtn, controlsBtn;
    [SerializeField] List<OptionScreen> screens = new List<OptionScreen>();

    MenuManager menu = null;

    public OptionScreen currentScreen = new OptionScreen();
    private GameObject lastSelected = null;

    public Vector2 currentRes;
    public Vector2 desiredRes;

    // For Changes    
    [SerializeField] TMP_Dropdown resDrop;
    [SerializeField] Toggle fullscreenTog;
    [SerializeField] Toggle vsyncTog;
    [SerializeField] Toggle motionBlurTog;



    private void OnEnable()
    {
        MainOptionButtonsInteract(true);

        ClearAndSelect(audioBtn);        

        menu = MenuManager.Instance;

        currentRes.x = Screen.width;
        currentRes.y = Screen.height;
    }

    private void Update()
    {
        if(Gamepad.current.bButton.wasPressedThisFrame)
        {
            if(currentScreen.type != OPTIONS_TYPE.NONE)
            {
                ChangeScreens(-1);
                MainOptionButtonsInteract(true);

                if(lastSelected != null)
                {
                    ClearAndSelect(lastSelected);
                }
                else
                {
                    ClearAndSelect(audioBtn);
                }
                
            }
            else
            {
                menu.OptionsMenu(false);
            }
        }
    }

    public void ChangeScreens(int type)
    {
        // Turn off all screens
        for (int i = 0; i < screens.Count; i++)
        {
            screens[i].screen.SetActive(false);
        }

        // Turn on current screen
        if ((OPTIONS_TYPE)type != OPTIONS_TYPE.NONE)
        {
            // Set last selected
            lastSelected = EventSystem.current.currentSelectedGameObject;

            // Change current Screen
            currentScreen = screens.Find((s) => s.type == (OPTIONS_TYPE)type);

            //screens.Find((s) => s.type == currentScreen.type).screen.SetActive(true);
            currentScreen.screen.SetActive(true);

            // Disable interaction of main buttons
            MainOptionButtonsInteract(false);

            // Select first option in new screen
            ClearAndSelect(currentScreen.defaultSelect);
        }
        else
        {
            currentScreen = new OptionScreen();
        }
    }

    public void MainOptionButtonsInteract(bool state)
    {
        audioBtn.GetComponent<Button>().interactable = state;
        videoBtn.GetComponent<Button>().interactable = state;
        controlsBtn.GetComponent<Button>().interactable = state;
    }

    public void ClearAndSelect(GameObject selection = null)
    {
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(selection);
    }

    public void TurnMenuOff()
    {
        // Turn off all screens
        for (int i = 0; i < screens.Count; i++)
        {
            screens[i].screen.SetActive(false);
        }

        this.gameObject.SetActive(false);
    }


    // Changes
    public void ApplyChanges()
    {
        Screen.fullScreen = fullscreenTog.isOn;

        //ADD MOTION BLUR

        Screen.SetResolution((int)desiredRes.x, (int)desiredRes.y, fullscreenTog.isOn);

        if(vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void PickNewRes()
    {
        string res = resDrop.options[resDrop.value].text.ToString();


        Debug.Log(res);

        if(res != "")
        {
            var newRes = res.Split("x"[0]);

            desiredRes.x = float.Parse(newRes[0]);
            desiredRes.y = float.Parse(newRes[1]);
        }
        else
        {
            desiredRes = currentRes;
        }
    }
}


[Serializable]
public class OptionScreen
{
    public GameObject screen = null;
    public OPTIONS_TYPE type = OPTIONS_TYPE.NONE;
    public GameObject defaultSelect = null;
}