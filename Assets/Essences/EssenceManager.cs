using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EssenceManager : MonoBehaviour
{
    AudioManager audioManager;
    public delegate void EssenceChanged();

    private Dictionary<EssenceColor, int> essenceCounts = new Dictionary<EssenceColor, int>
    {
        { EssenceColor.Red, 0 },
        { EssenceColor.Yellow, 0 },
        { EssenceColor.Blue, 0 },
        { EssenceColor.Purple, 0 },
        { EssenceColor.Green, 0 },
        { EssenceColor.Orange, 0 }
    };

    public event EssenceChanged OnEssenceChanged;

    private EssenceSpawner essenceSpawner;

    void Start()
    {
        essenceSpawner = FindFirstObjectByType<EssenceSpawner>();
    }

    public void CollectEssence(Essence essence, GameObject obj)
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        // Проверяем, есть ли у игрока сложная эссенция (фиолетовая, зелёная или оранжевая)
        if (essenceCounts[EssenceColor.Purple] > 0 || 
            essenceCounts[EssenceColor.Green] > 0 || 
            essenceCounts[EssenceColor.Orange] > 0)
        {
            Debug.Log("У вас уже есть сложная эссенция. Нельзя собрать больше.");
            if (gameManager != null)
            {
                gameManager.loserWindowByIncorrectEssence.SetActive(true); // Показываем окно поражения из-за неправильной эссенции
                Time.timeScale = 0; // Останавливаем время
            }
            return;
        }

        // Считаем количество базовых эссенций (красная, синяя, жёлтая)
        int basicEssenceCount = essenceCounts[EssenceColor.Red] +
                                essenceCounts[EssenceColor.Yellow] +
                                essenceCounts[EssenceColor.Blue];

        // Если у игрока уже две базовые эссенции, нельзя собрать больше
        if (basicEssenceCount >= 2 && 
            (essence.color == EssenceColor.Red || 
             essence.color == EssenceColor.Yellow || 
             essence.color == EssenceColor.Blue))
        {
            Debug.Log("У вас уже есть две базовые эссенции. Нельзя собрать больше.");
            return;
        }

        // Проверяем, есть ли у игрока уже эссенция этого цвета
        if (essenceCounts.TryGetValue(essence.color, out int count) && count > 0)
        {
            Debug.Log($"У вас уже есть эссенция цвета {essence.color}. Нельзя собрать одинаковые эссенции.");
            if (gameManager != null)
            {
                gameManager.loserWindowByIncorrectEssence.SetActive(true); // Показываем окно поражения из-за неправильной эссенции
                Time.timeScale = 0; // Останавливаем время
            }
            return;
        }

        // Добавляем эссенцию игроку
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.PlaySFX(audioManager.сollectedEssence);

        Debug.Log($"Собрана эссенция: {essence.color}");
        essenceCounts[essence.color]++;
        ScoreController.score += 10;

        // Проверяем, существует ли EssenceSpawner
        if (essenceSpawner != null)
        {
            essenceSpawner.DestroyAndSpawnEssence(obj);
        }
        else
        {
            Debug.LogWarning("EssenceSpawner не найден или был уничтожен.");
        }

        // Вызываем событие обновления UI
        OnEssenceChanged?.Invoke();

        // Проверяем возможность создания комбинации
        CheckForColorCombination();
    }

    public bool UseOrangeEssence()
    {
        if (essenceCounts[EssenceColor.Orange] > 0)
        {
            essenceCounts[EssenceColor.Orange]--;
            Debug.Log($"Оранжевое зелье использовано.");
            OnEssenceChanged?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetEssenceCount(EssenceColor color) => essenceCounts.TryGetValue(color, out int count) ? count : 0;

    private void CheckForColorCombination()
    {
        CombineEssences(EssenceColor.Red, EssenceColor.Yellow, EssenceColor.Orange);
        CombineEssences(EssenceColor.Yellow, EssenceColor.Blue, EssenceColor.Green);
        CombineEssences(EssenceColor.Blue, EssenceColor.Red, EssenceColor.Purple);
    }

    private void CombineEssences(EssenceColor color1, EssenceColor color2, EssenceColor resultColor)
    {
        if (essenceCounts[color1] > 0 && essenceCounts[color2] > 0)
        {
            audioManager = FindObjectOfType<AudioManager>();
            audioManager.PlaySFX(audioManager.сollectedPotion);
            essenceCounts[color1]--;
            essenceCounts[color2]--;
            essenceCounts[resultColor]++;
            Debug.Log($"Создано зелье: {resultColor}");
            OnEssenceChanged?.Invoke();
        }
    }
}