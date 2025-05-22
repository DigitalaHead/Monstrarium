using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause1 : MonoBehaviour
{
    // Start is called before the first frame update
    public bool PauseGame;
    public GameObject pauseGameMenu;
    public GameObject trainingMenu;
    public GameObject trainingMenu1;
    public GameObject trainingMenu2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PauseGame) 
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }

    public void Resume()
    {
        pauseGameMenu.SetActive(false);
        Time.timeScale = 1f;
        PauseGame = false;
    }

    public void Pause()
    {
        pauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
    }

    public void PauseTraining()
    {
        trainingMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
    }
    public void ResumeTraining() 
    {
        trainingMenu.SetActive(false);
        trainingMenu1.SetActive(false);
        trainingMenu2.SetActive(false);
        Time.timeScale = 1f;
        PauseGame = false;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
