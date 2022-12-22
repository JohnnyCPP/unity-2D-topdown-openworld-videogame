using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    public static CanvasController instance;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public Vector2 playerStartingPosition;
    public Vector2Value storedPlayerPosition;

    public bool gameEnded;


    private void Awake()
    {
        Time.timeScale = 1;
    }


    void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused())
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void StartGame()
    {
        storedPlayerPosition.startingPositionOnLoad = playerStartingPosition;
        SceneManager.LoadScene("Alpha");
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public bool GamePaused() {
        return pausePanel.activeInHierarchy;
    }
}
