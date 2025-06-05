using UnityEngine;
using UnityEngine.UI;

public class ComicManager : MonoBehaviour
{
    public GameObject comicPanel;      // Панель с комиксом
    public Image[] comicImages;        // Картинки комикса
    public GameObject[] tutorialObjects; // Объекты обучения

    private int currentIndex = 0;
    private bool comicActive = false;
    private bool tutorialActive = false;
    private bool lastSlideActive = false; // Новый флаг

    void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("ComicShown", 0));
        //PlayerPrefs.SetInt("ComicShown", 0); // Сброс для тестирования, удалить в релизе
        if (PlayerPrefs.GetInt("ComicShown", 0) == 0)
        {
            comicPanel.SetActive(true);
            ShowComicImage(0);
            Time.timeScale = 0;
            comicActive = true;
            tutorialActive = false;
        }
        else
        {
            comicPanel.SetActive(false);
            comicActive = false;
            tutorialActive = false;
        }
    }

    void Update()
    {
        if (comicActive && Input.anyKeyDown)
        {
            NextComicImage();
        }
        else if (tutorialActive && Input.anyKeyDown && !lastSlideActive)
        {
            NextTutorialObject();
        }
        else if (lastSlideActive && Input.anyKeyDown)
        {
            CloseAll();
        }
    }

    void ShowComicImage(int index)
    {
        for (int i = 0; i < comicImages.Length; i++)
            comicImages[i].gameObject.SetActive(i == index);
        // Скрыть все tutorialObjects
        foreach (var obj in tutorialObjects)
            obj.SetActive(false);
    }

    void ShowTutorialObject(int index)
    {
        for (int i = 0; i < tutorialObjects.Length; i++)
            tutorialObjects[i].SetActive(i == index);
        // Скрыть все comicImages
        foreach (var img in comicImages)
            img.gameObject.SetActive(false);

        // Проверяем, последний ли это слайд
        lastSlideActive = (index == tutorialObjects.Length - 1);
    }

    void NextComicImage()
    {
        currentIndex++;
        if (currentIndex < comicImages.Length)
        {
            ShowComicImage(currentIndex);
        }
        else
        {
            // Переход к обучению
            currentIndex = 0;
            comicActive = false;
            tutorialActive = true;
            ShowTutorialObject(0);
        }
    }

    void NextTutorialObject()
    {
        if (currentIndex < tutorialObjects.Length - 1)
        {
            currentIndex++;
            ShowTutorialObject(currentIndex);
            lastSlideActive = (currentIndex == tutorialObjects.Length - 1);
        }
        // Если уже последний слайд, просто ждём нажатия для CloseAll() в Update
    }

    void CloseAll()
    {
        comicPanel.SetActive(false);
        Time.timeScale = 1;
        PlayerPrefs.SetInt("ComicShown", 1);
        PlayerPrefs.Save();
        comicActive = false;
        tutorialActive = false;
        lastSlideActive = false; // сбрасываем флаг
    }
}