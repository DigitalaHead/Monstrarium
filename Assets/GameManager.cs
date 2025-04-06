using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public GameObject pacman;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeStart;
    public GameObject ghostNodeCenter;

    public GameObject loserWindowOne;
    public GameObject loserWindowTwo;

    // Метод для перезапуска текущего уровня
    public void Restart()
    {
        Time.timeScale = 1; // Возобновляем время
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезапускаем текущую сцену
    }

    // Метод для выхода в главное меню
    public void Exit()
    {
        // Проверяем, добавлена ли сцена в Build Settings
        if (Application.CanStreamedLevelBeLoaded("menu"))
        {
            SceneManager.LoadScene("menu"); // Загружаем сцену "menu"
        }
        else
        {
            Debug.Log("Сцена 'menu' не добавлена в Build Settings. Проверьте настройки сборки.");
        }
    }

    public void ShowGameOverMenu()
    {
        if (loserWindowTwo != null)
        {
            loserWindowTwo.SetActive(true); // Активируем окно поражения
            Time.timeScale = 0; // Останавливаем время в игре
        }
    }

    public void HideGameOverMenu()
    {
        if (loserWindowTwo != null)
        {
            loserWindowTwo.SetActive(false); // Скрываем окно поражения
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

    void Awake()
    {
        // ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
    }

    void Update()
    {
        
    }
}
