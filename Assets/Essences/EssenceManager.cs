using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EssenceManager : MonoBehaviour
{
    public delegate void EssenceChanged();

    private Dictionary<EssenceColor, int> essenceCounts = new Dictionary<EssenceColor, int>
    {
        { EssenceColor.Red, 0 },
        { EssenceColor.Yellow, 0 },
        { EssenceColor.Blue, 0 },
        { EssenceColor.Orange, 0 },
        { EssenceColor.Green, 0 },
        { EssenceColor.Purple, 0 },
        { EssenceColor.Burgundy, 0 }, // Бордовое зелье
        { EssenceColor.Mustard, 0 }, // Горчичное зелье
        { EssenceColor.Murena, 0 }   // Мурена
    };

    public event EssenceChanged OnEssenceChanged;

    private EssenceSpawner essenceSpawner;

    public GameObject effect; // Ссылка на объект эффекта

    void Start()
    {
        essenceSpawner = FindFirstObjectByType<EssenceSpawner>();
    }

    public void CollectEssence(Essence essence, GameObject obj)
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        // Если игрок собирает черную эссенцию
        if (essence.color == EssenceColor.Black)
        {
            Debug.Log("Черная эссенция собрана! Все зелья и эссенции очищены.");
            ClearAllEssences(); // Очищаем все эссенции и зелья
            Destroy(obj); // Удаляем черную эссенцию с карты
            OnEssenceChanged?.Invoke(); // Обновляем UI
            return;
        }

        // Проверяем, есть ли у игрока сложное зелье (бордовое, горчичное или мурена)
        if (essenceCounts[EssenceColor.Burgundy] > 0 || 
            essenceCounts[EssenceColor.Mustard] > 0 || 
            essenceCounts[EssenceColor.Murena] > 0)
        {
            Debug.Log("У вас уже есть сложное зелье (бордовое, горчичное или мурена). Нельзя собирать эссенции.");
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

            // Активируем эффект
            ActivateEffect();

            return true;
        }
        else
        {
            return false;
        }
    }

    private void ActivateEffect()
    {
        if (effect != null)
        {
            effect.SetActive(true); // Включаем объект эффекта
            Animator animator = effect.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("PlayEffect"); // Запускаем анимацию
            }

            // Отключаем эффект через 2 секунды
            StartCoroutine(DisableEffectAfterDelay(2f));
        }
        else
        {
            Debug.LogWarning("Объект эффекта не назначен!");
        }
    }

    private System.Collections.IEnumerator DisableEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null)
        {
            effect.SetActive(false); // Отключаем объект эффекта
        }
    }

    public int GetEssenceCount(EssenceColor color) => essenceCounts.TryGetValue(color, out int count) ? count : 0;

    private void CheckForColorCombination()
    {
        // Существующие комбинации
        CombineEssences(EssenceColor.Red, EssenceColor.Yellow, EssenceColor.Orange); // Оранжевое зелье
        CombineEssences(EssenceColor.Yellow, EssenceColor.Blue, EssenceColor.Green); // Зеленое зелье
        CombineEssences(EssenceColor.Blue, EssenceColor.Red, EssenceColor.Purple);  // Фиолетовое зелье

        // Новые комбинации
        CombineEssences(EssenceColor.Purple, EssenceColor.Yellow, EssenceColor.Burgundy); // Бордовое зелье
        CombineEssences(EssenceColor.Orange, EssenceColor.Red, EssenceColor.Mustard);    // Горчичное зелье
        CombineEssences(EssenceColor.Green, EssenceColor.Blue, EssenceColor.Murena);     // Мурена
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

    private void ClearAllEssences()
    {
        foreach (var color in essenceCounts.Keys.ToList())
        {
            essenceCounts[color] = 0; // Обнуляем количество всех эссенций
        }
        Debug.Log("Все эссенции и зелья очищены.");
    }
}