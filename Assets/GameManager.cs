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

    public GameObject loserWindowByIncorrectEssence; // Окно поражения из-за неправильной эссенции
    public GameObject loserWindowByMonster; // Окно поражения от монстра
    public GameObject winnerWindow; // Окно победы

    public GameObject menu; // Главное меню

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

    void Awake()
    {
        // ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
    }

    void Update()
    {
        
    }
}
