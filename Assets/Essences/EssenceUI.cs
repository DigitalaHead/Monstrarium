using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class EssenceUI : MonoBehaviour
{
    public EssenceManager essenceManager; // Ссылка на ваш EssenceManager
    public TextMeshProUGUI score; // Текст для отображения текущего счёта
    public TextMeshProUGUI highScore; // Текст для отображения рекорда

    // Словарь для хранения текстовых полей по цветам эссенций
    private Dictionary<EssenceColor, TextMeshProUGUI> essenceTextFields = new Dictionary<EssenceColor, TextMeshProUGUI>();

    private void Start()
    {
        essenceManager = FindFirstObjectByType<EssenceManager>();

        if (essenceManager != null)
        {
            essenceManager.OnEssenceChanged += UpdateUI; // Подписка на событие
        }
        else
        {
            Debug.LogError("EssenceManager не найден в сцене!");
        }

        // Сбрасываем счёт при начале новой игры
        ScoreController.score = 0;

        // Инициализация словаря с текстовыми полями
        InitializeTextFields();

        // Обновляем UI
        UpdateUI();
    }

    private void InitializeTextFields()
    {
        // Здесь можно инициализировать текстовые поля для каждого цвета эссенции
    }

    public void UpdateUI()
    {
        // Обновляем текст для каждой эссенции
        foreach (var color in essenceTextFields.Keys)
        {
            if (essenceTextFields[color] != null)
            {
                essenceTextFields[color].text = essenceManager.GetEssenceCount(color).ToString();
            }
        }

        // Обновляем текущий счёт
        if (score != null)
            score.text = ScoreController.score.ToString();

        // Обновляем рекорд
        if (highScore != null)
        {
            int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
            if (ScoreController.score > savedHighScore)
            {
                PlayerPrefs.SetInt("HighScore", ScoreController.score);
                PlayerPrefs.Save();
            }
            highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
    }
}