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
    // Start is called before the first frame update

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

    // Update is called once per frame
    void Update()
    {
        if (testRespawn == true) 
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


            if (transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }

            else if (transform.position.x == ghostNodeCenter.transform.position.x && transform.position.y == ghostNodeCenter.transform.position.y)
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

            else if ((transform.position.x == ghostNodeLeft.transform.position.x && transform.position.y == ghostNodeLeft.transform.position.y)
                 || (transform.position.x == ghostNodeRight.transform.position.x && transform.position.y == ghostNodeRight.transform.position.y))
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
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";

        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        // If we can move up and we aren't reversing
        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            // Get the node above us
            GameObject nodeUp = nodeController.nodeUp;
            // Get the distance between our top node and Pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);

            // If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            // Get the node below us
            GameObject nodeDown = nodeController.nodeDown;
            // Get the distance between our top node and Pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);

            // If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            // Get the node to the left of us
            GameObject nodeLeft = nodeController.nodeLeft;
            // Get the distance between our top node and Pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);

            // If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            // Get the node to the right of us
            GameObject nodeRight = nodeController.nodeRight;
            // Get the distance between our top node and Pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);

            // If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
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
            Destroy(collision.gameObject); // Уничтожаем игрока
            FindFirstObjectByType<GameManager>().ShowGameOverMenu(); // Показываем меню поражения
        }
    }


    IEnumerator RespawnGhost()
    {
        yield return new WaitForSeconds(2f); // �������� ����� ��������� (��������, 2 �������)

        // ������� ����� ������ �������� �� ��������� �������
        GameObject respawnedGhost = Instantiate(gameObject, startingNode.transform.position, Quaternion.identity);

        // ��������������� ��������� ��������� ��� ������������� ��������
        respawnedGhost.GetComponent<EnemyController>().ResetGhost();
    }

    public void ResetGhost()
    {
        ghostNodeState = GhostNodeStatesEnum.respawning;
        transform.position = startingNode.transform.position;
        movementController.currentNode = startingNode;

        // ��������������� ��������� ��������� ��� ������ ����� ���������
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
