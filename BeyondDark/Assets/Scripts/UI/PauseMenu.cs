using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject continueBtn, optionsBtn, mainMenuBtn, ExitGameBtn;

    [SerializeField] GameObject loadScreen;

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
        loadScreen.SetActive(true);
        loadScreen.GetComponent<LoadingScreen>().LoadNewScene(LoadingScreen.SCENES.MENU);
        MenuManager.Instance.PauseMenuGO.SetActive(false);
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
