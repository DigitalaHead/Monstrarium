using System.Collections;
using UnityEngine;

public class EssenceSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject redEssencePrefab;
    [SerializeField]
    private GameObject yellowEssencePrefab;
    [SerializeField]
    private Transform essencesParent;

    [SerializeField]
    private float respawnTime = 5f; // Время до респауна эссенции

    void Start()
    {
        InitializeEssences();
    }

    // Инициализация эссенций на старте
    private void InitializeEssences()
    {
        bool hasRed = false;
        bool hasYellow = false;

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
            }

            // Удаляем старую эссенцию и создаём новую
            ReplaceEssence(child);
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

    // Возвращает случайный префаб эссенции
    private GameObject GetRandomEssencePrefab()
    {
        if (redEssencePrefab == null || yellowEssencePrefab == null)
        {
            Debug.LogError("Префабы эссенций не назначены в инспекторе!");
            return null;
        }

        return Random.Range(0f, 1f) < 0.5f ? redEssencePrefab : yellowEssencePrefab;
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