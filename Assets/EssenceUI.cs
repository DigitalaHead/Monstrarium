using UnityEngine;
using UnityEngine.UI; // Не забудьте добавить этот namespace для работы с UI
using TMPro;

public class EssenceUI : MonoBehaviour
{
    public EssenceManager essenceManager; // Ссылка на ваш EssenceManager

    public TextMeshProUGUI redCount; // Ссылка на текстовое поле для отображения количества красных эссенций
    public TextMeshProUGUI blueCount; // Ссылка на текстовое поле для отображения количества синих эссенций

    public TextMeshProUGUI yellowCount;

    private void Start()
    {
        essenceManager = FindFirstObjectByType<EssenceManager>();

        if (essenceManager != null)
        {
            essenceManager.OnEssenceChanged += UpdateUI; // Подписка на событие
            Debug.Log("EssenceManager найден и подписка на событие выполнена.");
        }
        else
        {
            Debug.LogError("EssenceManager не найден в сцене!");
        }

        UpdateUI();
    }

    public void UpdateUI()
{
    if (redCount != null)
        redCount.text = essenceManager.redEssenceCount.ToString();

    if (yellowCount != null)
        yellowCount.text = essenceManager.yellowEssenceCount.ToString();

    if (blueCount != null)
        blueCount.text = essenceManager.blueEssenceCount.ToString();

    // Добавьте аналогичные проверки для других цветов
}
}