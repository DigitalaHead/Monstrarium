using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using MonsterType = EnemyController.MonsterType;
using Debug = UnityEngine.Debug;

public class GameManager : Sounds
{
    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public GameObject pacman;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeStart;
    public GameObject ghostNodeCenter;

    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;

    [Header("Ghost Prefabs")]
    public GameObject fireMonsterPrefab;
    public GameObject aquaMonsterPrefab;
    public GameObject woodMonsterPrefab;
    public GameObject ceramicMonsterPrefab;
    public GameObject electroMonsterPrefab;
    public GameObject sandMonsterPrefab;

    [SerializeField] private float soundInterval = 15f;
    private Coroutine soundRoutine;

    void Awake()
    {

        currentGhostMode = GhostMode.chase;
        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
    }

    // Метод для получения префаба по типу
    public GameObject GetGhostPrefab(MonsterType type)
    {
        // Вспомогательная функция для случайного выбора
        GameObject GetRandomPrefab(GameObject prefabA, GameObject prefabB)
        {
            if (prefabA == null) return prefabB;
            if (prefabB == null) return prefabA;
            return UnityEngine.Random.Range(0, 2) == 0 ? prefabA : prefabB;
        }

        switch (type)
        {
            case MonsterType.red:
                return fireMonsterPrefab;
            case MonsterType.aqua:
                return aquaMonsterPrefab;
            case MonsterType.wood:
                return GetRandomPrefab(woodMonsterPrefab, ceramicMonsterPrefab);
            case MonsterType.electro:
                return GetRandomPrefab(electroMonsterPrefab, sandMonsterPrefab);
            default:
                return fireMonsterPrefab;
        }
    }


    public enum GhostMode
    {
        chase, scatter
    }

    public GhostMode currentGhostMode;

    public GameObject loserWindowByIncorrectEssence; // Окно поражения из-за неправильной эссенции
    public GameObject loserWindowByMonster; // Окно поражения от монстра
    public GameObject winnerWindow; // Окно победы

    public GameObject menu; // Главное меню

    public EnemyController enemyController; // Ссылка на EnemyController



    void Start()
    {
        enemyController = FindFirstObjectByType<EnemyController>(); // Находим EnemyController в сцене
        StartRandomSoundCycle();
    }

    public void StartRandomSoundCycle()
    {
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
        soundRoutine = StartCoroutine(RandomSoundCycle());
    }

    private IEnumerator RandomSoundCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(soundInterval);
            PlayRandomSound(); // Метод из родительского класса
        }
    }

    public void SetSoundInterval(float newInterval)
    {
        soundInterval = newInterval;
        StartRandomSoundCycle(); // Перезапускаем с новым интервалом
    }

    // Метод для перезапуска текущего уровня
    public void Restart()
    {
        Time.timeScale = 1; // Возобновляем время
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезапускаем текущую сцену
    }

    // Метод для выхода в главное меню
    public void ExitMenu()
    {
        if (menu!= null)
            {
                menu.SetActive(true); // Активируем окно победы
                Time.timeScale = 0; // Останавливаем время в игре
            }
    }

    public void ShowGameOverMenu(bool playerWins)
    {
        if (playerWins)
        {
            Debug.Log("Игрок победил красного монстра!");

            if (winnerWindow != null)
            {
                winnerWindow.SetActive(true); // Активируем окно победы
                Time.timeScale = 0; // Останавливаем время в игре
            }
            else
            {
                Debug.LogWarning("Окно победы не назначено в инспекторе!");
            }
        }
        else
        {
            if (loserWindowByMonster != null)
            {
                loserWindowByMonster.SetActive(true); // Активируем окно поражения
                Time.timeScale = 0; // Останавливаем время в игре
            }
            else
            {
                Debug.LogWarning("Окно поражения не назначено в инспекторе!");
            }
        }
    }

    public void HideGameOverMenu()
    {
        if (loserWindowByMonster != null)
        {
            loserWindowByMonster.SetActive(false); // Скрываем окно поражения
            Time.timeScale = 1; // Возобновляем время
        }
    }

    // Пример кода для перемещения игрока на исходное положение
    public void ResetPlayerPosition(GameObject player, Vector3 startPosition)
    {
        player.transform.position = startPosition; // Перемещаем игрока
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Сбрасываем скорость
        player.SetActive(true); // Убедитесь, что объект активен
    }

    public void ResetPlayer(GameObject player, Vector3 startPosition)
    {
        player.transform.position = startPosition;
        player.SetActive(true); // Включаем объект игрока
    }

    public void ContinueGame()
    {
        // Скрываем окна победы и поражения
        if (winnerWindow != null)
        {
            winnerWindow.SetActive(false);
        }

        // Возобновляем время
        Time.timeScale = 1;

        // Сбрасываем состояние всех монстров
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            if (enemy != null) // Проверяем, что объект не уничтожен
            {
                StartCoroutine(enemy.RespawnGhost()); // Запускаем респавн для каждого монстра
            }
            else
            {
                Debug.LogWarning("Попытка респавна уничтоженного монстра.");
            }
        }

        Debug.Log("Игра продолжена, монстры сброшены.");
    }

   /* void Awake()
    {
        
        currentGhostMode = GhostMode.chase;
        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
    }*/

    void Update()
    {
        
    }
}
