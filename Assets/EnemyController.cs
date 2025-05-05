using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Cache;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool CollectionCombinations = false;

    public enum GhostNodeStatesEnum
    {
        respawning,
        redGhost,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes
    }

    public GhostNodeStatesEnum ghostNodeState;
    public GhostNodeStatesEnum respawnState;

    public enum MonsterType
    {
        fireOrAqua,
        blue,
        pink,
        orange
    }

    public MonsterType monsterType;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeStart;
    public GameObject ghostNodeCenter;

    public MovementController movementController;
    public GameObject startingNode;

    public bool readyToLeaveHome = false;
    public GameManager gameManager;
    public bool testRespawn = false;

    public bool isFrightened = false;

    public GameObject[] scatterNodes;
    public int scatterNodeIndex;

    void Awake()
    {
        scatterNodeIndex = 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();

        if (monsterType == MonsterType.fireOrAqua)
        {
            ghostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
            readyToLeaveHome = true;
            
        }
        else if (monsterType == MonsterType.pink)
        {
            ghostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeStatesEnum.centerNode;
        }
        else if (monsterType == MonsterType.blue)
        {
            ghostNodeState = GhostNodeStatesEnum.leftNode;
            respawnState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
        }
        else if (monsterType == MonsterType.orange)
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
            //режим разброса для указания по нему точного маршрута
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {

                if(transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
                {
                    scatterNodeIndex++;

                    if (scatterNodeIndex == scatterNodes.Length - 1)
                    {
                        scatterNodeIndex = 0;
                    }
                }

                string direction = GetClosestDirection(scatterNodes[scatterNodeIndex].transform.position);
                movementController.SetDirection(direction);
               
            }

            //режим неопределенности
            else if (isFrightened)
            {
                string direction = GetRandomDirection();
                movementController.SetDirection(direction);
            }

            // режим погони
            else
            {
                if (monsterType == MonsterType.fireOrAqua)
                {
                    DetermineRedGhostDirection();
                }

                else if (monsterType == MonsterType.pink)
                {
                    DeterminePinkGhostDirection();
                }

                else if (monsterType == MonsterType.blue)
                {
                    DetermineBlueGhostDirection();
                }

                else if (monsterType == MonsterType.orange)
                {
                    DetermineOrangeGhostDirection();
                }

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
                    movementController.SetDirection("left");
                }
            }
        }
    }

    void DetermineOrangeGhostDirection()
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        float distanceBetweenNodes = 0.17f;
        if (distance < 0)
        {
            distance *= -1;
        }

        // If we are within 8 nodes of pacman, chase him using red's logic
        if (distance <= distanceBetweenNodes * 10)
        {
            DetermineRedGhostDirection();
        }
        // Otherwise, use scatter mode logic
        else
        {
            // Scatter mode
            string direction = GetRandomDirection();
            movementController.SetDirection(direction);
        }
    }

    void DetermineBlueGhostDirection()
    {
        string playerDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetWeenNode = 0.17f;

        Vector2 target = gameManager.pacman.transform.position;
        if (playerDirection == "left")
        {
            target.x -= distanceBetWeenNode * 5;
        }
        else if (playerDirection == "right")
        {
            target.x += distanceBetWeenNode * 5;
        }
        else if (playerDirection == "up")
        {
            target.y += distanceBetWeenNode * 5;
        }
        else if (playerDirection == "down")
        {
            target.y -= distanceBetWeenNode * 5;
        }

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2 (target.x + xDistance, target.y + yDistance);
        string direction = GetClosestDirection (blueTarget);
        movementController.SetDirection(direction);

    }

    void DeterminePinkGhostDirection()
    {
        string playerDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetWeenNode = 0.17f;

        Vector2 target = gameManager.pacman.transform.position;
        if (playerDirection == "left")
        {
            target.x -= distanceBetWeenNode * 5;
        }
        else if (playerDirection == "right") 
        {
            target.x += distanceBetWeenNode * 5;
        }
        else if (playerDirection == "up")
        {
            target.y += distanceBetWeenNode * 5;
        }
        else if (playerDirection == "down")
        {
            target.y -= distanceBetWeenNode * 5;
        }
        string direction = GetClosestDirection(target);
        movementController.SetDirection(direction);
    }

    void DetermineRedGhostDirection()
    {
        // Определяем координаты игрока
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

    string GetRandomDirection()
    {
        List<string> possibleDirections = new List<string>();
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (nodeController.canMoveDown && movementController.lastMovingDirection != "up")
        {
            possibleDirections.Add("down");
        }
        if (nodeController.canMoveUp && movementController.lastMovingDirection != "down")
        {
            possibleDirections.Add("up");
        }
        if (nodeController.canMoveRight && movementController.lastMovingDirection != "left")
        {
            possibleDirections.Add("right");
        }
        if (nodeController.canMoveLeft && movementController.lastMovingDirection != "right")
        {
            possibleDirections.Add("left");
        }

        string direction = "";
        int randomDirectionIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
        direction = possibleDirections[randomDirectionIndex];
        return direction;
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
                movementController.lastMovingDirection = "down";
                movementController.direction = "down";
                ghostNodeState = GhostNodeStatesEnum.respawning;
                StartCoroutine(RespawnGhost());

                // Начисляем 50 очков за убийство монстра
                ScoreController.score += 50;
                Debug.Log("Монстр убит! +50 очков.");
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

    
}