using DG.Tweening;
using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public enum OPTIONS_TYPE { NONE = -1, AUDIO = 0, VIDEO = 1, CONTROLS = 2, DEBUG = 3 }

public class OptionsMenu : MonoBehaviour
{
    enum AUDIO_TYPE { MASTER = 0, EFFECTS = 1, MUSIC = 2, DIALOGUE = 3 }

    [Header("Menu Settings")]

    [SerializeField] GameObject audioBtn;
    [SerializeField] GameObject videoBtn;
    [SerializeField] GameObject controlsBtn;
    [SerializeField] GameObject debugBtn;
    [SerializeField] List<OptionScreen> screens = new List<OptionScreen>();

    MenuManager menu = null;
    public bool IsMainMenu = false;

    public OptionScreen currentScreen = new OptionScreen();
    private GameObject lastSelected = null;

    public Vector2 currentRes;
    public Vector2 desiredRes;

    [Header("Video Settings")]
    // For Video Changes    
    [SerializeField] TMP_Dropdown resDrop;
    [SerializeField] Toggle fullscreenTog;
    [SerializeField] Toggle vsyncTog;
    [SerializeField] Toggle motionBlurTog;


    [Header("Audio Settings")]
    // Audio
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider effectsSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider dialogueSlider;
    [SerializeField] AudioMixer masterMixer;

    [Header("Controls Settings")]
    [SerializeField] GameObject controlsMR;
    [SerializeField] GameObject controlsVR;
    [SerializeField] TextMeshProUGUI realmTxt;
    [SerializeField] Slider sensSlider;
    [SerializeField] CameraController cam;



    private void OnEnable()
    {
        MainOptionButtonsInteract(true);

        ClearAndSelect(audioBtn);        

        menu = MenuManager.Instance;

        currentRes.x = Screen.width;
        currentRes.y = Screen.height;

        // Initialize
        GetVolumes();
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

    public void TurnMenuOff(bool isMainMenu = false)
    {
        // Turn off all screens
        for (int i = 0; i < screens.Count; i++)
        {
            screens[i].screen.SetActive(false);
        }

        IsMainMenu = isMainMenu;
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

    public void SetMasterVolume(int type)
    {
        switch((AUDIO_TYPE)type)
        {
            case AUDIO_TYPE.MASTER:
                masterMixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20);
                PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
                break;
            case AUDIO_TYPE.EFFECTS:
                masterMixer.SetFloat("EffectsVolume", Mathf.Log10(effectsSlider.value) * 20);
                PlayerPrefs.SetFloat("EffectsVolume", effectsSlider.value);
                break;
            case AUDIO_TYPE.MUSIC:
                masterMixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);
                PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
                break;
            case AUDIO_TYPE.DIALOGUE:
                masterMixer.SetFloat("DialogueVolume", Mathf.Log10(dialogueSlider.value) * 20);
                PlayerPrefs.SetFloat("DialogueVolume", dialogueSlider.value);
                break;
        }
    }

    //Initialize
    public void GetVolumes()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 1);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        dialogueSlider.value = PlayerPrefs.GetFloat("DialogueVolume", 1);

        masterMixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20);
        masterMixer.SetFloat("EffectsVolume", Mathf.Log10(effectsSlider.value) * 20);
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);
        masterMixer.SetFloat("DialogueVolume", Mathf.Log10(dialogueSlider.value) * 20);

        //Sens
        sensSlider.value = PlayerPrefs.GetFloat("Sensitivity", 1);
    }

    public void ChangeControlsImage()
    {
        if(controlsMR.activeSelf)
        {
            controlsMR.SetActive(false);
            controlsVR.SetActive(true);

            realmTxt.text = "Vorgon's Realm";
        }
        else
        {
            controlsMR.SetActive(true);
            controlsVR.SetActive(false);
            realmTxt.text = "Mortal Realm";
        }
    }

    public void ChangeSensitivity()
    {
        cam.sensitivity = new Vector2(sensSlider.value, sensSlider.value);
        PlayerPrefs.SetFloat("Sensitivity", sensSlider.value);
    }
}


[Serializable]
public class OptionScreen
{
    public GameObject screen = null;
    public OPTIONS_TYPE type = OPTIONS_TYPE.NONE;
    public GameObject defaultSelect = null;
}