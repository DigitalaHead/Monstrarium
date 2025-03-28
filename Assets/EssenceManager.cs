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

    public void CollectEssence(Essence essence, GameObject obj)
    {
        if (essenceCounts.Values.Sum() <= 1 && essenceCounts[EssenceColor.Green] == 0 
            && essenceCounts[EssenceColor.Purple] == 0 && essenceCounts[EssenceColor.Orange] == 0)
            {
            if (essenceCounts[essence.color] == 0) // Проверка на наличие эссенции данного цвета
            {
                Debug.Log("Собрана эссенция: " + essence.color);
                essenceCounts[essence.color] += 1;
                ScoreController.score += 10;
                Destroy(obj);
            }
            else
            {
                Debug.Log("Уже есть эссенция цвета " + essence.color + ", не собираем.");
            }
        }
        else {
            Debug.Log("У вас уже много эссенций");
        }
        OnEssenceChanged?.Invoke();
        CheckForColorCombination();
    }

    public int GetEssenceCount(EssenceColor color)
    {
        if (essenceCounts.TryGetValue(color, out int count))
        {
            return count;
        }
        return 0; // Возвращаем 0, если цвет не найден
    }

    private void CheckForColorCombination()
    {
        // Пример: смешивание двух красных и одной желтой дает оранжевую
        if (essenceCounts[EssenceColor.Red] >= 1 && essenceCounts[EssenceColor.Yellow] >= 1)
        {
            CreateNewEssence(EssenceColor.Orange);
            essenceCounts[EssenceColor.Red] -= 1;
            essenceCounts[EssenceColor.Yellow] -= 1;
        }

        // Пример: смешивание двух желтых и одной синей дает зеленую
        if (essenceCounts[EssenceColor.Yellow] >= 1 && essenceCounts[EssenceColor.Blue] >= 1)
        {
            CreateNewEssence(EssenceColor.Green);
            essenceCounts[EssenceColor.Yellow] -= 1;
            essenceCounts[EssenceColor.Blue] -= 1;
        }

        // Пример: смешивание двух синих и одной красной дает фиолетовую
        if (essenceCounts[EssenceColor.Blue] >= 1 && essenceCounts[EssenceColor.Red] >= 1)
        {
            CreateNewEssence(EssenceColor.Purple);
            essenceCounts[EssenceColor.Blue] -= 1;
            essenceCounts[EssenceColor.Red] -= 1;
        }

        OnEssenceChanged?.Invoke();
    }

    private void CreateNewEssence(EssenceColor newColor)
    {
        Debug.Log("Создано зелье: " + newColor);
        essenceCounts[newColor] += 1;
        OnEssenceChanged?.Invoke();
        // Здесь можно добавить логику для создания новой эссенции в игре
    }
}