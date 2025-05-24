using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class EssenceUI : MonoBehaviour
{
    public List<TextMeshProUGUI> scoreTexts; // Основной список текстовых полей для отображения счета
    public List<TextMeshProUGUI> highScoreTexts = new List<TextMeshProUGUI>(); // Список текстовых полей для отображения рекорда
    public EssenceManager essenceManager; // Ссылка на EssenceManager

    private EssenceCheckmark[] essenceCheckmarks;

    private void Start()
    {
        ScoreController.score = 0; // Сброс текущего счета при запуске

        essenceCheckmarks = FindObjectsOfType<EssenceCheckmark>();

        if (essenceManager != null)
        {
            essenceManager.OnEssenceChanged += UpdateUI; // Подписка на событие
        }
        else
        {
            Debug.LogError("EssenceManager не найден в сцене!");
        }

        // Загружаем рекорд из PlayerPrefs
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        UpdateUI(); // Обновляем UI при старте
    }

    private void SynchronizeHighScoreTexts()
    {
        // Синхронизируем длину списка highScoreTexts с длиной scoreTexts
        while (highScoreTexts.Count < scoreTexts.Count)
        {
            highScoreTexts.Add(null); // Добавляем пустые элементы
        }

        while (highScoreTexts.Count > scoreTexts.Count)
        {
            highScoreTexts.RemoveAt(highScoreTexts.Count - 1); // Удаляем лишние элементы
        }
    }

    public void UpdateUI()
    {
        UpdateCheckmarks();
        UpdateScoreTexts(ScoreController.score);
        UpdateHighScore();
    }

    public void UpdateCheckmarks()
    {
        foreach (var checkmark in essenceCheckmarks)
        {
            if (checkmark == null)
            {
                Debug.LogWarning("Обнаружен null в essenceCheckmarks!");
                continue;
            }

            if (essenceManager == null)
            {
                Debug.LogError("EssenceManager не назначен!");
                return;
            }

            // Проверяем, есть ли зелье соответствующего цвета
            bool hasEssence = essenceManager.HasEssence(checkmark.essenceColor);
            checkmark.UpdateCheckmark(hasEssence);
        }
    }

    private void UpdateScoreTexts(int newScore)
    {
        foreach (var scoreText in scoreTexts)
        {
            if (scoreText != null)
            {
                scoreText.text = newScore.ToString(); // Обновляем текст
                scoreText.ForceMeshUpdate(); // Принудительное обновление
            }
        }
    }

    private void UpdateHighScore()
    {
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (ScoreController.score > savedHighScore)
        {
            PlayerPrefs.SetInt("HighScore", ScoreController.score);
            PlayerPrefs.Save();
            Debug.Log($"Новый рекорд: {ScoreController.score}");
        }

        // Обновляем текст рекорда в любом случае
        UpdateHighScoreTexts();
    }

    private void UpdateHighScoreTexts()
    {
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        for (int i = 0; i < highScoreTexts.Count; i++)
        {
            if (highScoreTexts[i] != null)
            {
                highScoreTexts[i].text = savedHighScore.ToString();
                highScoreTexts[i].ForceMeshUpdate(); // Принудительное обновление
            }
            else
            {
                Debug.LogWarning($"Текстовое поле {i} равно null!");
            }
        }
    }
}