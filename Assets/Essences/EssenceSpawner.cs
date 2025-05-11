using System.Collections;
using UnityEngine;

public class EssenceSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject redEssencePrefab;
    [SerializeField]
    private GameObject yellowEssencePrefab;
    [SerializeField]
    private GameObject blueEssencePrefab; // Префаб синей эссенции
    [SerializeField]
    private GameObject blackEssencePrefab; // Префаб черной эссенции
    [SerializeField]
    private Transform essencesParent;
    [SerializeField]
    private Transform blackEssenceSpawnPoint; // Точка появления черной эссенции

    [SerializeField]
    private float respawnTime = 5f; // Время до респауна эссенции
    [SerializeField]
    private float blackEssenceSpawnInterval = 60f; // Интервал появления черной эссенции

    private GameObject currentBlackEssence; // Ссылка на текущую черную эссенцию

    void Start()
    {
        InitializeEssences();
        StartCoroutine(SpawnBlackEssenceRoutine());
    }

    // Инициализация эссенций на старте
    private void InitializeEssences()
    {
        // Флаги для проверки наличия эссенций каждого цвета
        bool hasRed = false;
        bool hasYellow = false;
        bool hasBlue = false;

        // Проверяем существующие эссенции
        for (int i = essencesParent.childCount - 1; i >= 0; i--)
        {
            Transform child = essencesParent.GetChild(i);
            EssenceController essenceController = child.GetComponent<EssenceController>();

            if (essenceController != null)
            {
                if (essenceController.Color == EssenceColor.Red)
                    hasRed = true;
                else if (essenceController.Color == EssenceColor.Yellow)
                    hasYellow = true;
                else if (essenceController.Color == EssenceColor.Blue)
                    hasBlue = true;
            }
        }

        // Убедимся, что на карте есть хотя бы одна красная эссенция
        if (!hasRed)
        {
            Instantiate(redEssencePrefab, GetRandomPosition(), Quaternion.identity, essencesParent);
            Debug.Log("Добавлена красная эссенция, так как её не было на карте.");
        }

        // Убедимся, что на карте есть хотя бы одна жёлтая эссенция
        if (!hasYellow)
        {
            Instantiate(yellowEssencePrefab, GetRandomPosition(), Quaternion.identity, essencesParent);
            Debug.Log("Добавлена жёлтая эссенция, так как её не было на карте.");
        }

        // Убедимся, что на карте есть хотя бы одна синяя эссенция
        if (!hasBlue)
        {
            Instantiate(blueEssencePrefab, GetRandomPosition(), Quaternion.identity, essencesParent);
            Debug.Log("Добавлена синяя эссенция, так как её не было на карте.");
        }
    }

    // Заменяет существующую эссенцию на новую
    private void ReplaceEssence(Transform oldEssence)
    {
        GameObject randomPrefab = GetRandomEssencePrefab();
        if (randomPrefab == null)
        {
            Debug.LogError("Префабы эссенций не назначены в инспекторе!");
            return;
        }

        Vector3 position = oldEssence.position;
        Collider2D collider = oldEssence.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false; // Отключаем коллайдер
        }
        Destroy(oldEssence.gameObject); // Удаляем старую эссенцию
        Instantiate(randomPrefab, position, Quaternion.identity, essencesParent); // Создаём новую
    }

    // Спауним новую эссенцию
    public void SpawnEssences(Vector3 position)
    {
        GameObject essenceToSpawn = GetRandomEssencePrefab();
        if (essenceToSpawn == null)
        {
            Debug.LogError("Префабы эссенций не назначены в инспекторе!");
            return;
        }

        Instantiate(essenceToSpawn, position, Quaternion.identity, essencesParent);
    }

    // Удаляет старую эссенцию и запускает корутину для спауна новой
    public void DestroyAndSpawnEssence(GameObject essence)
    {
        if (essence == null)
        {
            Debug.LogWarning("Essence is null. Cannot destroy or spawn.");
            return;
        }

        Vector3 position = essence.transform.position;
        Destroy(essence); // Удаляем старую эссенцию
        StartCoroutine(SpawnEssenceAfterDelay(position, respawnTime)); // Запускаем корутину для спауна
    }

    // Корутин для задержки перед спауном
    private IEnumerator SpawnEssenceAfterDelay(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay); // Ждём указанное время
        SpawnEssences(position); // Спауним новую эссенцию
    }

    // Корутин для спауна черной эссенции
    private IEnumerator SpawnBlackEssenceRoutine()
    {
        while (true)
        {
            // Ждём заданный интервал
            yield return new WaitForSeconds(blackEssenceSpawnInterval);

            // Если черной эссенции нет, создаём её
            if (currentBlackEssence == null)
            {
                currentBlackEssence = Instantiate(blackEssencePrefab, blackEssenceSpawnPoint.position, Quaternion.identity);
                Debug.Log("Черная эссенция появилась!");
            }
        }
    }

    // Возвращает случайный префаб эссенции
    private GameObject GetRandomEssencePrefab()
    {
        if (redEssencePrefab == null || yellowEssencePrefab == null || blueEssencePrefab == null)
        {
            Debug.LogError("Префабы эссенций не назначены в инспекторе!");
            return null;
        }

        int randomIndex = Random.Range(0, 3); // Убираем синюю эссенцию из выбора
        if (randomIndex == 0) return redEssencePrefab;
        if (randomIndex == 1) return yellowEssencePrefab;
        return blueEssencePrefab;
    }

    // Возвращает случайную позицию в пределах родительского объекта
    private Vector3 GetRandomPosition()
    {
        if (essencesParent.childCount > 0)
        {
            Transform randomChild = essencesParent.GetChild(Random.Range(0, essencesParent.childCount));
            return randomChild.position;
        }

        // Если нет дочерних объектов, возвращаем позицию родителя
        return essencesParent.position;
    }
}