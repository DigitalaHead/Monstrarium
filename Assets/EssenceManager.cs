using UnityEngine;
using System.Collections.Generic;

public class EssenceManager : MonoBehaviour
{
    private int redEssenceCount = 0;
    private int yellowEssenceCount = 0;
    private int blueEssenceCount = 0;
    

    public void CollectEssence(Essence essence, GameObject obj)
    {
        switch (essence.color)
        {
            case EssenceColor.Red:
                if (redEssenceCount == 0) // Проверка на наличие красной эссенции
                {
                    Debug.Log("Собрана эссенция: " + essence.color);
                    redEssenceCount += 1;
                    Destroy(obj);
                }
                else
                {
                    Debug.Log("Уже есть красная эссенция, не собираем.");
                }
                break;
            case EssenceColor.Yellow:
                if (yellowEssenceCount == 0) // Проверка на наличие желтой эссенции
                {
                    Debug.Log("Собрана эссенция: " + essence.color);
                    yellowEssenceCount += 1;
                    Destroy(obj);
                }
                else
                {
                    Debug.Log("Уже есть желтая эссенция, не собираем.");
                }
                break;
            case EssenceColor.Blue:
                if (blueEssenceCount == 0) // Проверка на наличие синей эссенции
                {
                    Debug.Log("Собрана эссенция: " + essence.color);
                    blueEssenceCount += 1;
                    Destroy(obj);
                }
                else
                {
                    Debug.Log("Уже есть синяя эссенция, не собираем.");
                }
                break;
        }
        CheckForColorCombination();
    }

    private void CheckForColorCombination()
    {
        // Пример: смешивание двух красных и одной желтой дает оранжевую
        if (redEssenceCount == 1 && yellowEssenceCount == 1)
        {
            CreateNewEssence(EssenceColor.Orange);
            redEssenceCount -= 1;
            yellowEssenceCount -= 1;
        }

        // Пример: смешивание двух желтых и одной синей дает зеленую
        if (yellowEssenceCount == 1 && blueEssenceCount == 1)
        {
            CreateNewEssence(EssenceColor.Green);
            yellowEssenceCount -= 1;
            blueEssenceCount -= 1;
        }

        // Пример: смешивание двух синих и одной красной дает фиолетовую
        if (blueEssenceCount == 1 && redEssenceCount == 1)
        {
            CreateNewEssence(EssenceColor.Purple);
            blueEssenceCount -= 1;
            redEssenceCount -= 1;
        }
    }

    private void CreateNewEssence(EssenceColor newColor)
    {
        Debug.Log("Создана новая эссенция цвета: " + newColor);
        // Здесь можно добавить логику для создания новой эссенции в игре
    }
}