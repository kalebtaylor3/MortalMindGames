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
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        //SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
        //
        //StartCoroutine(SwitchScenes());
    }

    IEnumerator SwitchScenes()
    {
        SceneManager.UnloadSceneAsync("KalebMilestone4");
        
        yield return null;
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);

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
