using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject continueBtn, optionsBtn, mainMenuBtn, ExitGameBtn;

    MenuManager menu = null;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(continueBtn);

        menu = MenuManager.Instance;
    }
    
    public void ContinueGame()
    {
        menu.PauseGame();
    }

    public void Options()
    {
        menu.OptionsMenu(true);
    }

    public void MainMenu()
    {
        Debug.Log("Go to Main Menu");
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
