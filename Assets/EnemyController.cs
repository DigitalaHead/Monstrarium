using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool CollectionCombinations = false;

    public enum GhostNodeStatesEnum
    {
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public GhostNodeStatesEnum ghostNodeState;
    public GhostNodeStatesEnum respawnState;

    public enum GhostType
    {
        red,
        blue,
        pink,
        orange
    }

    public GhostType ghostType;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeStart;
    public GameObject ghostNodeCenter;

    public MovementController movementController;
    public GameObject startingNode;

    public bool readyToLeaveHome = false;
    public GameManager gameManager;
    public bool testRespawn = false;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();

        if (ghostType == GhostType.red)
        {
            ghostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
            readyToLeaveHome = true;
        }
        else if (ghostType == GhostType.pink)
        {
            ghostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeStatesEnum.centerNode;
        }
        else if (ghostType == GhostType.blue)
        {
            ghostNodeState = GhostNodeStatesEnum.leftNode;
            respawnState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
        }
        else if (ghostType == GhostType.orange)
        {
            ghostNodeState = GhostNodeStatesEnum.rightNode;
            respawnState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
        }

        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;
    }

    void Update()
    {
        if (testRespawn)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodeStatesEnum.respawning;
            testRespawn = false;
        }
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodeStatesEnum.movingInNodes)
        {
            if (ghostType == GhostType.red)
            {
                DetermineRedGhostDirection();
            }
        }
        else if (ghostNodeState == GhostNodeStatesEnum.respawning)
        {
            string direction = "";

            if (transform.position == ghostNodeStart.transform.position)
            {
                direction = "down";
            }
            else if (transform.position == ghostNodeCenter.transform.position)
            {
                if (respawnState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodeStatesEnum.leftNode)
                {
                    direction = "left";
                }
                else if (respawnState == GhostNodeStatesEnum.rightNode)
                {
                    direction = "right";
                }
            }
            else if (transform.position == ghostNodeLeft.transform.position || transform.position == ghostNodeRight.transform.position)
            {
                ghostNodeState = respawnState;
            }
            else
            {
                direction = GetClosestDirection(ghostNodeStart.transform.position);
            }

            movementController.SetDirection(direction);
        }
        else
        {
            if (readyToLeaveHome)
            {
                if (ghostNodeState == GhostNodeStatesEnum.leftNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("right");
                }
                else if (ghostNodeState == GhostNodeStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("left");
                }
                else if (ghostNodeState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.startNode;
                    movementController.SetDirection("up");
                }
                else if (ghostNodeState == GhostNodeStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                }
            }
        }
    }

    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }

    string GetClosestDirection(Vector2 target)
    {
        float shortestDistance = float.MaxValue;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            GameObject nodeUp = nodeController.nodeUp;
            float distance = Vector2.Distance(nodeUp.transform.position, target);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            GameObject nodeDown = nodeController.nodeDown;
            float distance = Vector2.Distance(nodeDown.transform.position, target);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            GameObject nodeLeft = nodeController.nodeLeft;
            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            GameObject nodeRight = nodeController.nodeRight;
            float distance = Vector2.Distance(nodeRight.transform.position, target);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EssenceManager essenceManager = FindFirstObjectByType<EssenceManager>();

            if (essenceManager != null && essenceManager.UseOrangeEssence())
            {
                // Если у игрока есть оранжевая эссенция, монстр респавнится
                readyToLeaveHome = false;
                transform.position = ghostNodeCenter.transform.position;
                movementController.currentNode = ghostNodeCenter;
                ghostNodeState = GhostNodeStatesEnum.respawning;
                StartCoroutine(RespawnGhost());
            }
            else
            {
                // Если у игрока нет оранжевой эссенции, игрок проигрывает
                collision.gameObject.SetActive(false); // Отключаем объект игрока вместо уничтожения
                if (gameManager.loserWindowByMonster != null)
                {
                    gameManager.loserWindowByMonster.SetActive(true); // Показываем окно поражения
                }
                else
                {
                    Debug.LogWarning("Окно поражения не назначено в инспекторе!");
                }
            }
        }
    }

    public IEnumerator RespawnGhost()
    {
        yield return new WaitForSeconds(2f);
        readyToLeaveHome = true;
    }

    public void ResetGhost()
    {
        ghostNodeState = GhostNodeStatesEnum.respawning;
        transform.position = startingNode.transform.position;
        movementController.currentNode = startingNode;

        if (ghostType == GhostType.red)
        {
            ghostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
            readyToLeaveHome = true;
        }
        else if (ghostType == GhostType.pink)
        {
            ghostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeStatesEnum.centerNode;
        }
        else if (ghostType == GhostType.blue)
        {
            ghostNodeState = GhostNodeStatesEnum.leftNode;
            respawnState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
        }
        else if (ghostType == GhostType.orange)
        {
            ghostNodeState = GhostNodeStatesEnum.rightNode;
            respawnState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
        }

        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;
    }
}