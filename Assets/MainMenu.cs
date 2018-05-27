using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject endGameMenu;
    public GameObject endGameTitle;

    private void Awake()
    {
        if (GameManager.instance != null)
            SetupEndGameMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
        if (GameManager.instance != null)
            GameManager.instance.StartNewGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetupEndGameMenu()
    {
        var title = endGameTitle.GetComponent<TextMeshProUGUI>();
        title.text = GameManager.GetWinnerTeam().ToString().ToUpper() + " TEAM WINS";
        mainMenu.SetActive(false);
        endGameMenu.SetActive(true);
    }
}
