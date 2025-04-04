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

    private float respawnTime = 5f;

    void Start()
    {
        InitializeEssences();
    }

    private void InitializeEssences()
    {
        // Список для отслеживания существующих цветов
        bool hasRed = false;
        bool hasYellow = false;

        // Проходим по всем дочерним объектам essencesParent
        for (int i = essencesParent.childCount - 1; i >= 0; i--)
        {
            Transform child = essencesParent.GetChild(i);

            // Проверяем цвет текущей эссенции
            EssenceController essenceController = child.GetComponent<EssenceController>();
            if (essenceController != null)
            {
                if (essenceController.Color == EssenceColor.Red)
                    hasRed = true;
                else if (essenceController.Color == EssenceColor.Yellow)
                    hasYellow = true;
            }

            // Выбираем случайный цвет
            GameObject randomPrefab = GetRandomEssencePrefab();

            // Создаем новую эссенцию на месте старой
            Vector3 position = child.position;
            Destroy(child.gameObject); // Удаляем старую эссенцию
            Instantiate(randomPrefab, position, Quaternion.identity, essencesParent);
        }

        // Убедимся, что на карте есть хотя бы одна красная эссенция
        if (!hasRed)
        {
            Instantiate(redEssencePrefab, GetRandomPosition(), Quaternion.identity, essencesParent);
            Debug.Log("Добавлена красная эссенция, так как ее не было на карте.");
        }

        // Убедимся, что на карте есть хотя бы одна желтая эссенция
        if (!hasYellow)
        {
            Instantiate(yellowEssencePrefab, GetRandomPosition(), Quaternion.identity, essencesParent);
            Debug.Log("Добавлена желтая эссенция, так как ее не было на карте.");
        }
    }

    public void SpawnEssences(Vector3 position)
    {
        GameObject essenceToSpawn = GetRandomEssencePrefab();
        if (essenceToSpawn == null)
        {
            Debug.LogError("Essence prefabs are not assigned in the inspector!");
            return;
        }

        GameObject newEssence = Instantiate(essenceToSpawn, position, Quaternion.identity, essencesParent);
        Debug.Log($"Создана новая эссенция: {newEssence.name}");
    }

    public void DestroyAndSpawnEssence(GameObject essence)
    {
        if (essence == null)
        {
            Debug.LogWarning("Essence is null. Cannot destroy or spawn.");
            return;
        }

        Vector3 position = essence.transform.position;
        Destroy(essence);
        StartCoroutine(SpawnEssenceAfterDelay(position, respawnTime));
    }

    private IEnumerator SpawnEssenceAfterDelay(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnEssences(position);
    }

    private GameObject GetRandomEssencePrefab()
    {
        return Random.Range(0f, 1f) < 0.5f ? redEssencePrefab : yellowEssencePrefab;
    }

    private Vector3 GetRandomPosition()
    {
        // Возвращает случайную позицию в пределах родительского объекта
        if (essencesParent.childCount > 0)
        {
            Transform randomChild = essencesParent.GetChild(Random.Range(0, essencesParent.childCount));
            return randomChild.position;
        }

        // Если нет дочерних объектов, возвращаем позицию родителя
        return essencesParent.position;
    }
}