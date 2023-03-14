using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject newGameBtn, optionsBtn, creditsBtn, ExitGameBtn;

    MenuManager menu = null;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(newGameBtn);

        menu = MenuManager.Instance;
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(newGameBtn);

        menu = MenuManager.Instance;
    }

    private void Update()
    {
        if (Gamepad.current.startButton.wasPressedThisFrame)
        {
            EventSystem.current.SetSelectedGameObject(null);

            EventSystem.current.SetSelectedGameObject(newGameBtn);
        }
    }

    public void NewGame()
    {
        
    }

    public void Options()
    {
        menu.OptionsMenu(true);
    }

    public void Credits()
    {
        Debug.Log("Show Credits");
    }

    public void ExitGame()
    {
        PlayerPrefs.Save();

        Application.Quit();


#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
