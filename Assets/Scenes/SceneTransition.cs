using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public int sceneIndex;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
