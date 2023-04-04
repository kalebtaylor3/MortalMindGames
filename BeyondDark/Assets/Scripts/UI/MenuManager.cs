using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    #region Instance

    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);
    }

    #endregion


    [SerializeField] public GameObject PauseMenuGO;    

    [SerializeField] GameObject OptionsMenuGO;

    [SerializeField] GameObject MainMenuGO;

    bool pause = false;
    public bool canPause = true;
    public bool mainMenu = false;

    private void OnEnable()
    {
        if(mainMenu)
        {
            Time.timeScale = 1;
        }
    }

    private void Start()
    {
        OptionsMenuGO.GetComponent<OptionsMenu>().GetVolumes();
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.startButton.wasPressedThisFrame && !mainMenu && canPause)
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pause = !pause;

        if (pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }        

        PauseMenuGO.SetActive(pause);        
        OptionsMenuGO.GetComponent<OptionsMenu>().TurnMenuOff();
        WorldData.Instance.SetPauseGame(pause);
    }

    public void OptionsMenu(bool state)
    {
        if(!mainMenu)
        {
            PauseMenuGO.SetActive(!state);
        }
        else
        {
            MainMenuGO.SetActive(!state);
        }
        
        OptionsMenuGO.GetComponent<OptionsMenu>().TurnMenuOff(mainMenu);
        OptionsMenuGO.SetActive(state);
    }
}
