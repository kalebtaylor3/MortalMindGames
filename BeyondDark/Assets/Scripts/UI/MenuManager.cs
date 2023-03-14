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


    [SerializeField] GameObject PauseMenuGO;    

    [SerializeField] GameObject OptionsMenuGO;

    bool pause = false;
    public bool mainMenu = false;

    private void Start()
    {
        OptionsMenuGO.GetComponent<OptionsMenu>().GetVolumes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Gamepad.current.startButton.wasPressedThisFrame && !mainMenu)
        {
            PauseGame();
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
        PauseMenuGO.SetActive(!state);
        OptionsMenuGO.GetComponent<OptionsMenu>().TurnMenuOff();
        OptionsMenuGO.SetActive(state);
    }
}
