using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EssenceManager : MonoBehaviour
{
    private Dictionary<EssenceColor, int> essenceCounts = new Dictionary<EssenceColor, int>
    {
        { EssenceColor.Red, 0 },
        { EssenceColor.Yellow, 0 },
        { EssenceColor.Blue, 0 },
        { EssenceColor.Purple, 0 },
        { EssenceColor.Green, 0 },
        { EssenceColor.Orange, 0 }
    };

    public delegate void EssenceChanged();
    public event EssenceChanged OnEssenceChanged;

    private EssenceSpawner essenceSpawner;

    void Start()
    {
        essenceSpawner = FindFirstObjectByType<EssenceSpawner>();
    }

    public void CollectEssence(Essence essence, GameObject obj)
    {
        // Проверяем, есть ли у игрока уже оранжевая, зеленая или фиолетовая эссенция
        if (essenceCounts[EssenceColor.Orange] > 0 || 
            essenceCounts[EssenceColor.Green] > 0 || 
            essenceCounts[EssenceColor.Purple] > 0)
        {
            Debug.Log("Инвентарь заполнен");
            return;
        }

        // Проверяем, есть ли у игрока уже эссенция этого цвета
        if (essenceCounts.TryGetValue(essence.color, out int count) && count > 0)
        {
            Debug.Log($"У вас уже есть эссенция цвета {essence.color}");
            return;
        }

        // Добавляем эссенцию игроку
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

        OnEssenceChanged?.Invoke();

        // Проверяем возможность создания комбинации
        CheckForColorCombination();
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
            essenceCounts[color1]--;
            essenceCounts[color2]--;
            essenceCounts[resultColor]++;
            Debug.Log($"Создано зелье: {resultColor}");
            OnEssenceChanged?.Invoke();
        }
    }
}