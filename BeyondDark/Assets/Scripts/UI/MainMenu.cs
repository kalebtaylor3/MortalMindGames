using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject newGameBtn, optionsBtn, creditsBtn, ExitGameBtn;
    [SerializeField] GameObject loadScreen, mainMenuScreen;

    MenuManager menu = null;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(newGameBtn);

        loadScreen.SetActive(false);
        mainMenuScreen.SetActive(true);

        menu = MenuManager.Instance;
    }

    private void Awake()
    {
        //EventSystem.current.SetSelectedGameObject(null);

        //EventSystem.current.SetSelectedGameObject(newGameBtn);

        //menu = MenuManager.Instance;
    }

    private void Start()
    {
        //EventSystem.current.SetSelectedGameObject(null);

        //EventSystem.current.SetSelectedGameObject(newGameBtn);

        //menu = MenuManager.Instance;
    }

    private void Update()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.startButton.wasPressedThisFrame)
            {
                EventSystem.current.SetSelectedGameObject(null);

                EventSystem.current.SetSelectedGameObject(newGameBtn);
            }
        }
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        //SceneManager.LoadSceneAsync("KalebMilestone4", LoadSceneMode.Single);
        loadScreen.SetActive(true);
        loadScreen.GetComponent<LoadingScreen>().LoadNewScene(LoadingScreen.SCENES.GAME);
        mainMenuScreen.SetActive(false);
        

        //StartCoroutine(SwitchScenes());
    }

    IEnumerator SwitchScenes()
    {
        SceneManager.UnloadSceneAsync("Main Menu");
        yield return null;
        
        SceneManager.LoadScene("KalebMilestone4", LoadSceneMode.Single);
    }

    public void Options()
    {
        menu.OptionsMenu(true);
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
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
