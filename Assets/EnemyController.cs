using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Cache;
using System.Security.Cryptography;
using UnityEngine;
using YG;
using Debug = UnityEngine.Debug;

public class EnemyController : MonoBehaviour
{
    public bool CollectionCombinations = false;

    public bool clone = false;

    public bool spriteRight = false;

    [Header("References")]
    public GameManager gameManager;

    AudioManager audioManager;

    public Animator animator;

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
        red,       // Огненный
        aqua,      // Водяной
        wood,      // Древесный
        electro,    // Электрический
        sand,      // Песчаный
        ceramic    // Керамический
    }

    public MonsterType monsterType;

    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeStart;
    public GameObject ghostNodeCenter;

    public MovementController movementController;
    public GameObject startingNode;

    public bool readyToLeaveHome = false;
    //public GameManager gameManager;
    public bool testRespawn = false;

    public bool isFrightened = false;

    public GameObject[] scatterNodes;
    public int scatterNodeIndex;

    public SpriteRenderer sprite;

    private bool _deathSoundPlayed = false;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        scatterNodeIndex = 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();

        if (clone)
        {
            ghostNodeStart = gameManager.ghostNodeStart;
            ghostNodeCenter = gameManager.ghostNodeCenter;
            ghostNodeRight = gameManager.ghostNodeRight;
            ghostNodeLeft = gameManager.ghostNodeLeft;

            // Устанавливаем состояние "respawning" для корректного выхода
            // ghostNodeState = GhostNodeStatesEnum.respawning;
            //respawnState = GhostNodeStatesEnum.movingInNodes; // Сразу перейдём в режим патрулирования
            readyToLeaveHome = true;


            // Начинаем движение из центра
            movementController.currentNode = ghostNodeCenter;
            transform.position = ghostNodeCenter.transform.position;

            ghostNodeState = GhostNodeStatesEnum.centerNode;

            // Направление вверх (как в оригинальном Pacman)

        }

        else
        {
            if (monsterType == MonsterType.red)
            {
                ghostNodeState = GhostNodeStatesEnum.startNode;
                respawnState = GhostNodeStatesEnum.centerNode;
                startingNode = ghostNodeStart;
                readyToLeaveHome = true;

            }
            else if (monsterType == MonsterType.wood)
            {
                ghostNodeState = GhostNodeStatesEnum.centerNode;
                startingNode = ghostNodeCenter;
                respawnState = GhostNodeStatesEnum.centerNode;
            }
            else if (monsterType == MonsterType.aqua)
            {
                ghostNodeState = GhostNodeStatesEnum.leftNode;
                respawnState = GhostNodeStatesEnum.leftNode;
                startingNode = ghostNodeLeft;
            }
            else if (monsterType == MonsterType.electro)
            {
                ghostNodeState = GhostNodeStatesEnum.rightNode;
                respawnState = GhostNodeStatesEnum.rightNode;
                startingNode = ghostNodeRight;
            }

            movementController.currentNode = startingNode;
            transform.position = startingNode.transform.position;
        }
    }

    void Update()
    {

        if (testRespawn)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodeStatesEnum.respawning;
            testRespawn = false;
        }

        //animator.SetBool("Moving", true);

        bool flipX = false;
        //bool flipY = false;

        if (spriteRight)
        {
            // if (movementController.lastMovingDirection == "right")
            // {
            //    animator.SetBool("Moving", true);
            // }

            if (movementController.lastMovingDirection == "left")
            {
                // animator.SetBool("Moving", true);
                flipX = true;
            }
        }
        else
        {
            //if (movementController.lastMovingDirection == "left")
            //{
            //  animator.SetBool("Moving", true);
            // }

            if (movementController.lastMovingDirection == "right")
            {
                //animator.SetBool("Moving", true);
                flipX = true;
            }
        }


        /*
        else if (movementController.lastMovingDirection == "up")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.lastMovingDirection == "down")
        {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipY = flipY;
        */
        sprite.flipX = flipX;
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {

        if (ghostNodeState == GhostNodeStatesEnum.movingInNodes)
        {
            //режим разброса для указания по нему точного маршрута
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {

                if (transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
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
                if (monsterType == MonsterType.red)
                {
                    DetermineRedGhostDirection();
                }

                else if (monsterType == MonsterType.wood)
                {
                    DeterminePinkGhostDirection();
                }

                else if (monsterType == MonsterType.aqua)
                {
                    DetermineBlueGhostDirection();
                }

                else if (monsterType == MonsterType.electro)
                {
                    DetermineOrangeGhostDirection();
                }

            }

        }
        else if (ghostNodeState == GhostNodeStatesEnum.respawning)
        {
            string direction = "";

            if (!clone && transform.position == ghostNodeStart.transform.position)
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
                    if (!clone) // Для обычных призраков
                    {
                        ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                        movementController.SetDirection("left");
                    }
                    else // Для клонов
                    {
                        ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                        // Оставляем текущее направление ("up")
                    }
                }
            }
        }
    }

    void DetermineOrangeGhostDirection()
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        float distanceBetweenNodes = 36f;

        // Если близко к игроку - преследуем как красный призрак
        if (distance <= distanceBetweenNodes * 5)
        {
            DetermineRedGhostDirection();
        }
        else // В режиме разброса
        {
            NodeController currentNode = movementController.currentNode.GetComponent<NodeController>();

            // Если мы в тупике - сразу разворачиваемся
            if (IsDeadEnd(currentNode))
            {
                movementController.SetDirection(ReverseDirection(movementController.lastMovingDirection));
            }
            else
            {
                // Иначе выбираем случайное направление
                string direction = GetRandomDirection();
                movementController.SetDirection(direction);
            }
        }
    }

    void DetermineBlueGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
        /*
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
        */

    }

    void DeterminePinkGhostDirection()
    {
        string playerDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetWeenNode = 36f;

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

    private bool IsDeadEnd(NodeController node)
    {
        if (node == null) return false;

        int exits = 0;
        if (node.canMoveLeft) exits++;
        if (node.canMoveRight) exits++;
        if (node.canMoveUp) exits++;
        if (node.canMoveDown) exits++;

        return exits == 1;
    }

    private NodeController GetNodeAtPosition(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapCircle(pos, 0.1f);
        return hit != null ? hit.GetComponent<NodeController>() : null;
    }


    string GetClosestDirection(Vector2 target)
    {
        NodeController currentNode = movementController.currentNode.GetComponent<NodeController>();
        NodeController targetNode = GetNodeAtPosition(target);
        string lastDirection = movementController.lastMovingDirection;

        bool targetInDeadEnd = IsDeadEnd(targetNode);
        Dictionary<string, float> directionScores = new Dictionary<string, float>();
        float deadEndBonus = targetInDeadEnd ? 50f : 0f;

        // Проверяем все возможные направления
        void EvaluateDirection(string direction, GameObject node)
        {
            if (node == null) return;

            NodeController nc = node.GetComponent<NodeController>();
            float distance = Vector2.Distance(node.transform.position, target);

            if (targetInDeadEnd && IsDeadEnd(nc))
                distance -= deadEndBonus;

            directionScores.Add(direction, distance);
        }

        if (currentNode.canMoveUp && lastDirection != "down")
            EvaluateDirection("up", currentNode.nodeUp);

        if (currentNode.canMoveDown && lastDirection != "up")
            EvaluateDirection("down", currentNode.nodeDown);

        if (currentNode.canMoveLeft && lastDirection != "right")
            EvaluateDirection("left", currentNode.nodeLeft);

        if (currentNode.canMoveRight && lastDirection != "left")
            EvaluateDirection("right", currentNode.nodeRight);

        return directionScores.Count > 0
            ? directionScores.OrderBy(x => x.Value).First().Key
            : ReverseDirection(lastDirection);
    }


    string ReverseDirection(string dir)
    {
        return dir switch
        {
            "up" => "down",
            "down" => "up",
            "left" => "right",
            "right" => "left",
            _ => dir
        };
    }

    string GetRandomDirection()
    {
        List<string> possibleDirections = new List<string>();
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();
        string lastDirection = movementController.lastMovingDirection;

        // Добавляем только допустимые направления
        if (nodeController.canMoveDown && lastDirection != "up")
            possibleDirections.Add("down");
        if (nodeController.canMoveUp && lastDirection != "down")
            possibleDirections.Add("up");
        if (nodeController.canMoveRight && lastDirection != "left")
            possibleDirections.Add("right");
        if (nodeController.canMoveLeft && lastDirection != "right")
            possibleDirections.Add("left");

        // Если нет возможных направлений (глухой тупик), разворачиваемся
        if (possibleDirections.Count == 0)
            return ReverseDirection(lastDirection);

        // Выбираем случайное направление из доступных
        return possibleDirections[UnityEngine.Random.Range(0, possibleDirections.Count)];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EssenceManager essenceManager = FindFirstObjectByType<EssenceManager>();
            if (essenceManager != null && essenceManager.IsShieldActive)
            {
                Debug.Log("Столкновение с игроком под щитом — монстр не атакует.");
                return;
            }

            if (essenceManager != null && essenceManager.TryKillMonster(monsterType))
            {
                Debug.Log($"Монстр {monsterType} убит!");
                // Можно проиграть звук убийства монстра, если нужно:
                // PlaySound(sounds[0]);

                //StartCoroutine(RespawnRandomGhost());
                Die();
                
            }
            else
            {

                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                if (playerController != null && !playerController.Dead)
                {

                    if (audioManager == null)
                    {
                        audioManager = FindObjectOfType<AudioManager>();
                    }
                    if (audioManager != null && audioManager.deathPlayer != null)
                    {
                        audioManager.PlaySFX(audioManager.deathPlayer);
                    }
                    else
                    {
                        Debug.LogWarning("AudioManager или звук deathPlayer не найден!");
                    }


                    if (playerController != null && !playerController.IsDead)
                    {
                        playerController.DieFromMonster();
                    }

                    playerController.KilledByMonster();
                    StartCoroutine(ShowLoseWindowWithDelay());
                }
            }
        }
    }



    public IEnumerator RespawnGhost()
    {
        yield return new WaitForSeconds(2f);
        readyToLeaveHome = true;
    }




   /* public IEnumerator RespawnSimpleGhost()
    {
        Debug.Log("SimpleGhost");
        // Сохраняем позицию для спавна
        Vector3 spawnPos = ghostNodeCenter != null ?
            ghostNodeCenter.transform.position :
            transform.position;

        // Отключаем визуал и коллайдеры
        SetActiveComponents(false);

        yield return new WaitForSeconds(2f);

        if (gameManager != null && gameManager.simpleGhostPrefab != null)
        {
            GameObject newGhost = Instantiate(
                gameManager.simpleGhostPrefab,
                spawnPos,
                Quaternion.identity
            );

            // Настраиваем нового монстра
            if (newGhost != null)
            {
                EnemyController newController = newGhost.GetComponent<EnemyController>();
                if (newController != null)
                {
                    newController.movementController.currentNode = ghostNodeCenter;
                    newController.ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                }
            }
        }

        Die();
    }*/

    private void SetActiveComponents(bool active)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = active;
        foreach (var collider in GetComponentsInChildren<Collider2D>())
            collider.enabled = active;
    }


    public IEnumerator RespawnRandomGhost()
    {
        // 1. Фиксируем позицию
        Vector3 spawnPos = ghostNodeCenter.transform.position;

        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }

        // 2. Проверяем и воспроизводим звук с защитой от ошибок
        if (audioManager != null && audioManager.deathPlayer != null)
        {
            audioManager.PlaySFX(audioManager.deathEnemy);
        }
        else
        {
            Debug.LogWarning("AudioManager или звук deathPlayer не найден!");
        }

        
        // 2. Отключаем ВСЕ SpriteRenderer и Collider2D (включая дочерние объекты)
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);

        foreach (var renderer in spriteRenderers)
        {
            renderer.enabled = false;
        }
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        // 3. Ждём 2 секунды
        yield return new WaitForSeconds(2f);

        // 4. Выбираем случайный тип призрака
        MonsterType randomType = (MonsterType)UnityEngine.Random.Range(0, 6);
        GameObject ghostPrefab = gameManager.GetGhostPrefab(randomType);

        // 5. Создаём нового призрака
        GameObject newGhost = Instantiate(ghostPrefab, spawnPos, Quaternion.identity);
        newGhost.SetActive(true);
    }

    /*
    public IEnumerator RespawnRandomGhost()
    {
        // 1. Фиксируем позицию спавна (центр)
        Vector3 spawnPos = ghostNodeCenter.transform.position;

        // 2. Отключаем визуал и коллайдер у старого монстра
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Collider2D collider = GetComponent<Collider2D>();

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (collider != null) collider.enabled = false;

        // 3. Ждём 2 секунды перед респавном
        yield return new WaitForSeconds(2f);

        // 4. Берём КРАСНОГО монстра (вместо случайного)
        GameObject newGhost = Instantiate(
            gameManager.fireMonsterPrefab, // Используем прямое обращение к префабу
            spawnPos,
            Quaternion.identity
        );

        // 5. Настраиваем компоненты нового монстра
        EnemyController newController = newGhost.GetComponent<EnemyController>();
        if (newController != null)
        {
            newController.ghostNodeState = GhostNodeStatesEnum.movingInNodes;
            newController.respawnState = GhostNodeStatesEnum.movingInNodes;
            newController.movementController.currentNode = ghostNodeCenter;
        }

        // 6. Уничтожаем старый объект
        Destroy(gameObject);
    }
    */

    private IEnumerator ShowLoseWindowWithDelay()
    {
        yield return new WaitForSeconds(3f); // Ждём 3 секунды

        if (gameManager.loserWindowByMonster != null)
        {
            gameManager.loserWindowByMonster.SetActive(true);
            Time.timeScale = 0;
            YG2.InterstitialAdvShow(); // Показываем рекламу
        }
    }

    public void Die()
    {
        Debug.Log("DIE вызван");
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("die");
        }
        if (movementController != null)
            movementController.enabled = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Destroy(gameObject, 2f); // Временно закомментируйте
    }
}