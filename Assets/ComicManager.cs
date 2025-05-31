using UnityEngine;
using UnityEngine.UI;

public class ComicManager : MonoBehaviour
{
    public GameObject comicPanel; // Панель с комиксом
    public Image[] comicImages;   // Массив картинок-комиксов

    private int currentIndex = 0;
    private bool comicActive = false;

    void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("ComicShown", 0));
        if (PlayerPrefs.GetInt("ComicShown", 0) == 0)
        {
            comicPanel.SetActive(true);
            ShowImage(0);
            Time.timeScale = 0;
            comicActive = true;
        }
        else
        {
            comicPanel.SetActive(false);
            comicActive = false;
        }
    }

    void Update()
    {
        if (comicActive && Input.anyKeyDown)
        {
            NextImage();
        }
    }

    void ShowImage(int index)
    {
        for (int i = 0; i < comicImages.Length; i++)
            comicImages[i].gameObject.SetActive(i == index);
    }

    void NextImage()
    {
        currentIndex++;
        if (currentIndex < comicImages.Length)
        {
            ShowImage(currentIndex);
        }
        else
        {
            CloseComic();
        }
    }

    public void CloseComic()
    {
        comicPanel.SetActive(false);
        Time.timeScale = 1;
        PlayerPrefs.SetInt("ComicShown", 1);
        PlayerPrefs.Save();
        comicActive = false;
    }
}