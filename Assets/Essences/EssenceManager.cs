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
        { EssenceColor.Orange, 0 },
        { EssenceColor.Green, 0 },
        { EssenceColor.Purple, 0 },
        { EssenceColor.Burgundy, 0 }, // Бордовое зелье
        { EssenceColor.Mustard, 0 }, // Горчичное зелье
        { EssenceColor.Murena, 0 }   // Мурена
    };

    public event EssenceChanged OnEssenceChanged;

    private EssenceSpawner essenceSpawner;


    public GameObject orangeEffect;
    public GameObject greenEffect;
    public GameObject purpleEffect;
    public GameObject burgundyEffect;
    public GameObject mustardEffect;
    public GameObject murenaEffect;

    public PlayerController player;

    private bool shieldActive = false;
    private float shieldTimer = 0f;
    public bool IsShieldActive => shieldActive;

    void Start()
    {
        essenceSpawner = FindFirstObjectByType<EssenceSpawner>();
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (shieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                shieldActive = false;
                Debug.Log("Щит закончился!");
                // Здесь можно отключить визуальный эффект щита, если есть
            }
        }
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
        if (essence.color == EssenceColor.Shield)
        {
            Debug.Log("Щит собран! Игрок неуязвим 10 секунд.");
            shieldActive = true;
            shieldTimer = 10f;
            Destroy(obj); // Удаляем щит с карты
            OnEssenceChanged?.Invoke();
            return;
        }
        // Проверяем, есть ли у игрока сложное зелье (бордовое, горчичное или мурена)
        if (essenceCounts[EssenceColor.Burgundy] > 0 || 
            essenceCounts[EssenceColor.Mustard] > 0 || 
            essenceCounts[EssenceColor.Murena] > 0)
        {
            Debug.Log("У вас уже есть сложное зелье (бордовое, горчичное или мурена). Нельзя собирать эссенции.");

            // Находим игрока и вызываем смерть с духом
            
            if (player != null && !player.IsDead)
            {
                player.DieFromWrongEssence(); // Только этот метод!
            }
            else
            {
                Debug.LogWarning("Игрок не найден или уже мёртв.");
            }

            // Окно поражения и Time.timeScale = 0 вызываются из корутины в PlayerController
            return;
        }
        
        // Считаем количество базовых эссенций (красная, синяя, жёлтая)
        int basicEssenceCount = essenceCounts[EssenceColor.Red] +
                                essenceCounts[EssenceColor.Yellow] +
                                essenceCounts[EssenceColor.Blue];


        // Проверяем, есть ли у игрока уже эссенция этого цвета
        if (essenceCounts.TryGetValue(essence.color, out int count) && count > 0)
        {
            Debug.Log($"У вас уже есть эссенция цвета {essence.color}. Нельзя собрать одинаковые эссенции.");
            if (player != null && !player.IsDead)
            {
                player.DieFromWrongEssence();
            }
            return;
        }

        // Добавляем эссенцию игроку
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.PlaySFX(audioManager.сollectedEssence);

        Debug.Log($"Собрана эссенция: {essence.color}");
        essenceCounts[essence.color]++;
        ScoreController.score += 10;
        OnEssenceChanged?.Invoke(); // Убедитесь, что событие вызывается
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

    public bool UseEssence(EssenceColor color)
    {
        if (essenceCounts[color] > 0)
        {
            essenceCounts[color]--;
            if (essenceCounts[color] == 0)
            {
                DisableEssenceEffect(color);
            }
            OnEssenceChanged?.Invoke(); // Обновляем UI
            return true;
        }
        return false;
    }

    public bool TryKillMonster(EnemyController.MonsterType monsterType)
    {
        EssenceColor requiredEssence = GetEssenceForMonster(monsterType);

        if (essenceCounts[requiredEssence] > 0)
        {
            essenceCounts[requiredEssence]--;
            if (essenceCounts[requiredEssence] == 0)
            {
                DisableEssenceEffect(requiredEssence);
            }
            Debug.Log($"Использовано зелье {requiredEssence} для убийства монстра {monsterType}.");

            // Добавляем 50 очков за убийство монстра
            ScoreController.score += 50;
            Debug.Log($"Счет увеличен на 50. Текущий счет: {ScoreController.score}");

            OnEssenceChanged?.Invoke(); // Обновляем UI
            return true;
        }

        return false;
    }

    private EssenceColor GetEssenceForMonster(EnemyController.MonsterType monsterType)
    {
        switch (monsterType)
        {
            case EnemyController.MonsterType.red:
                return EssenceColor.Orange; // Оранжевое зелье убивает огненного монстра
            case EnemyController.MonsterType.aqua:
                return EssenceColor.Murena; // Мурена убивает водяного монстра
            case EnemyController.MonsterType.wood:
                return EssenceColor.Green; // Зеленое зелье убивает древесного монстра
            case EnemyController.MonsterType.electro:
                return EssenceColor.Purple; // Фиолетовое зелье убивает электрического монстра
            case EnemyController.MonsterType.sand:
                return EssenceColor.Burgundy; // Бордовое зелье убивает песчаного монстра
            case EnemyController.MonsterType.ceramic:
                return EssenceColor.Mustard; // Горчичное зелье убивает керамического монстра
            default:
                Debug.LogWarning($"Неизвестный тип монстра: {monsterType}");
                return EssenceColor.Red; // Возвращаем значение по умолчанию
        }
    }

    private void ActivateEssenceEffect(EssenceColor color)
    {
        Debug.Log("ActivateEssenceEffect вызван для " + color);
        GameObject effect = null;
        switch (color)
        {
            case EssenceColor.Orange:   effect = orangeEffect; break;
            case EssenceColor.Green:    effect = greenEffect; break;
            case EssenceColor.Purple:   effect = purpleEffect; break;
            case EssenceColor.Burgundy: effect = burgundyEffect; break;
            case EssenceColor.Mustard:  effect = mustardEffect; break;
            case EssenceColor.Murena:   effect = murenaEffect; break;
            default:
                Debug.LogWarning("Нет эффекта для цвета: " + color);
                break;
        }
        if (effect != null)
        {
            effect.SetActive(true);
            var animator = effect.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
                animator.SetTrigger("PlayEffect");
            }
        }
    }

    private void DisableEssenceEffect(EssenceColor color)
    {
        switch (color)
        {
            case EssenceColor.Orange: orangeEffect.SetActive(false); break;
            case EssenceColor.Green: greenEffect.SetActive(false); break;
            case EssenceColor.Purple: purpleEffect.SetActive(false); break;
            case EssenceColor.Burgundy: burgundyEffect.SetActive(false); break;
            case EssenceColor.Mustard: mustardEffect.SetActive(false); break;
            case EssenceColor.Murena: murenaEffect.SetActive(false); break;
        }
    }

    public int GetEssenceCount(EssenceColor color) => essenceCounts.TryGetValue(color, out int count) ? count : 0;

    public bool HasEssence(EssenceColor essenceColor)
    {
        if (essenceCounts.ContainsKey(essenceColor))
        {
            return essenceCounts[essenceColor] > 0; // Проверяем, есть ли хотя бы одно зелье данного цвета
        }

        Debug.LogWarning($"Цвет зелья {essenceColor} не найден в словаре essenceCounts!");
        return false;
    }

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
            audioManager = FindObjectOfType<AudioManager>();
            audioManager.PlaySFX(audioManager.сollectedPotion);
            essenceCounts[color1]--;
            essenceCounts[color2]--;
            essenceCounts[resultColor]++;
            Debug.Log($"Создано зелье: {resultColor}");
            
            ActivateEssenceEffect(resultColor);
            OnEssenceChanged?.Invoke();
        }
    }

    private void ClearAllEssences()
    {
        foreach (var color in essenceCounts.Keys.ToList())
        {
            essenceCounts[color] = 0; // Обнуляем количество всех эссенций
        }
        DisableEssenceEffect(EssenceColor.Orange);
        DisableEssenceEffect(EssenceColor.Green);  
        DisableEssenceEffect(EssenceColor.Purple);
        DisableEssenceEffect(EssenceColor.Burgundy);
        DisableEssenceEffect(EssenceColor.Mustard);
        DisableEssenceEffect(EssenceColor.Murena);
        Debug.Log("Все эссенции и зелья очищены.");
        OnEssenceChanged?.Invoke();
    }
}