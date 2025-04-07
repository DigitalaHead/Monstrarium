using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public void ReStartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("_1Scene");
    }

    public void BackMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void GameOver()
    {
        Application.Quit();
    }
}
