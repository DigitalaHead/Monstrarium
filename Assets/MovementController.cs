using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MovementController : MonoBehaviour
{
    private float baseSpeed;
    private float currentSpeed;

    public GameManager gameManager;
    public GameObject currentNode;
    public float speed = 200f;
    public string direction = "";
    public string lastMovingDirection = "";

    public bool canWarp = true;

    public bool isGhost = false;
    // Start is called before the first frame update
    void Awake()
    {
        baseSpeed = speed; // Сохраняем изначальную скорость
        currentSpeed = baseSpeed;

        // Подписываемся на событие только если у объекта тег "Enemy"
        if (gameObject.CompareTag("Enemy"))
        {
            GameManager.OnSpeedIncreased += UpdateSpeed;
        }

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnDestroy()
    {
        // Отписываемся при уничтожении объекта
        if (gameObject.CompareTag("Enemy"))
        {
            GameManager.OnSpeedIncreased -= UpdateSpeed;
        }
    }

    private void UpdateSpeed(float multiplier)
    {
        currentSpeed = baseSpeed * multiplier;
        speed = currentSpeed; // Применяем новую скорость
        Debug.Log($"Скорость {gameObject.name} изменена: {speed}");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentNode == null)
        {
            Debug.LogError("currentNode не назначен в MovementController.");
            return;
        }

        NodeController currentNodeController = currentNode.GetComponent<NodeController>();
        if (currentNodeController == null)
        {
            Debug.LogError("NodeController не найден на объекте currentNode.");
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, currentNode.transform.position, speed * Time.deltaTime);
        bool reverseDirection = false;

        if (
           (direction == "left" && lastMovingDirection == "right")
           || (direction == "right" && lastMovingDirection == "left")
           || (direction == "up" && lastMovingDirection == "down")
           || (direction == "down" && lastMovingDirection == "up")
           )
        {
            reverseDirection = true;
        }

        if ((transform.position.x == currentNode.transform.position.x && transform.position.y == currentNode.transform.position.y) || reverseDirection)
        {
            if (isGhost)
            {
                GetComponent<EnemyController>().ReachedCenterOfNode(currentNodeController);
            }

            if (currentNodeController.isWarpLeftNode && canWarp)
            {
                currentNode = gameManager.rightWarpNode;
                direction = "left";
                lastMovingDirection = "left";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            else if (currentNodeController.isWarpRightNode && canWarp)
            {
                currentNode = gameManager.leftWarpNode;
                direction = "right";
                lastMovingDirection = "right";
                transform.position = currentNode.transform.position;
                canWarp = false;
            }
            else
            {
                if (currentNodeController.isGhostStartingNode && direction == "down"
                    && (!isGhost || GetComponent<EnemyController>().ghostNodeState != EnemyController.GhostNodeStatesEnum.respawning))
                {
                    direction = lastMovingDirection;
                }
                GameObject newNode = currentNodeController.GetNodeFromDirection(direction);

                if (newNode != null)
                {
                    currentNode = newNode;
                    lastMovingDirection = direction;
                }
                else
                {
                    direction = lastMovingDirection;
                    newNode = currentNodeController.GetNodeFromDirection(direction);
                    if (newNode != null)
                    {
                        currentNode = newNode;
                    }
                }
            }
        }
        else
        {
            canWarp = true;
        }
    }

    public void SetDirection(string newDirection)
    {
        direction = newDirection;
    }
}
