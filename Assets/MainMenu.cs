using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject endGameMenu;

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            mainMenu.SetActive(false);
            endGameMenu.SetActive(true);
        }
    }

    public void PlayGame ()
    {
        SceneManager.LoadScene("Game");
        if (GameManager.instance != null)
            GameManager.instance.StartNewGame();
    }
    
    public void QuitGame ()
    {
        Application.Quit();
    }
}
