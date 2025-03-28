using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class EssenceUI : MonoBehaviour
{
    public EssenceManager essenceManager; // Ссылка на ваш EssenceManager

     // Поля для текстовых компонентов
    public TextMeshProUGUI redCount; // Ссылка на текстовое поле для отображения количества красных эссенций
    public TextMeshProUGUI yellowCount; // Ссылка на текстовое поле для отображения количества желтых эссенций
    public TextMeshProUGUI blueCount; // Ссылка на текстовое поле для отображения количества синих эссенций
    public TextMeshProUGUI greenCount; // Ссылка на текстовое поле для отображения количества зеленых эссенций
    public TextMeshProUGUI orangeCount; // Ссылка на текстовое поле для отображения количества оранжевых эссенций
    public TextMeshProUGUI purpleCount; // Ссылка на текстовое поле для отображения количества фиолетовых эссенций

    public TextMeshProUGUI score;

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

        // Инициализация словаря с текстовыми полями
        InitializeTextFields();

        UpdateUI();
    }

    private void InitializeTextFields()
    {
        // Здесь вы можете добавить все текстовые поля в словарь
        essenceTextFields[EssenceColor.Red] = redCount;
        essenceTextFields[EssenceColor.Yellow] = yellowCount;
        essenceTextFields[EssenceColor.Blue] = blueCount;
        essenceTextFields[EssenceColor.Green] = greenCount;

        essenceTextFields[EssenceColor.Orange] = orangeCount;
        essenceTextFields[EssenceColor.Purple] = purpleCount;
    }

    public void UpdateUI()
    {
        foreach (var color in essenceTextFields.Keys)
        {
            if (essenceTextFields[color] != null)
            {
                essenceTextFields[color].text = essenceManager.GetEssenceCount(color).ToString();
            }
        }
        if (score != null)
            score.text = "Счёт: " + ScoreController.score;
    }
}