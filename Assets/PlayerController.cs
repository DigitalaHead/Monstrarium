using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movementController;
    private SpriteRenderer spriteRenderer;

    public Sprite deadBodySprite; // Лежачий спрайт героя
    public GameObject spiritPrefab; // Префаб духа (с нужным спрайтом)
    public Vector3 spiritOffset = Vector3.zero; // Смещение духа относительно тела (обычно (0,0,0))

    private bool isDead = false;

    public bool IsDead => isDead; // Свойство для проверки состояния игрока

    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movementController.SetDirection("left");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movementController.SetDirection("right");
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movementController.SetDirection("up");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movementController.SetDirection("down");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EssenceController essence = collision.GetComponent<EssenceController>();
        if (essence != null)
        {
            EssenceManager essenceManager = FindFirstObjectByType<EssenceManager>();
            if (essenceManager != null)
            {
                essenceManager.CollectEssence(essence.CreateEssence(), collision.gameObject);
            }
        }
    }

    public void DieFromMonster()
    {
        if (isDead) return;
        isDead = true;

        // Останавливаем движение через MovementController
        if (movementController != null)
        {
            movementController.SetDirection(null);
            movementController.enabled = false; // Полностью отключаем скрипт движения
        }

        // Останавливаем Rigidbody2D, если есть
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;

        StartCoroutine(DeathSequence());
        this.enabled = false;
    }

    private IEnumerator DeathSequence()
    {
        // 1. Меняем спрайт героя на лежачий
        if (spriteRenderer != null && deadBodySprite != null)
            spriteRenderer.sprite = deadBodySprite;

        // 2. Создаём духа в позиции героя
        GameObject spirit = null;
        if (spiritPrefab != null)
        {
            spirit = Instantiate(
                spiritPrefab,
                transform.position + spiritOffset,
                Quaternion.identity
            );
            var rb = spirit.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = true; // Отключаем физику
        }

        // 3. Двигаем духа вверх 3 секунды
        if (spirit != null)
        {
            Vector3 start = spirit.transform.position;
            Vector3 end = start + new Vector3(0, 150f, 0); 
            float duration = 3f; // <-- теперь 3 секунды
            float elapsed = 0f;
            while (elapsed < duration)
            {
                spirit.transform.position = Vector3.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            spirit.transform.position = end;
        }

        // Здесь можно добавить задержку и вызвать появление окна поражения
    }

    public void DieFromWrongEssence()
    {
        if (isDead) return;
        isDead = true;

        // Останавливаем движение
        if (movementController != null)
        {
            movementController.SetDirection(null);
            movementController.enabled = false;
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;

        StartCoroutine(DeathSequenceWithMenu());
        this.enabled = false;
    }

    private IEnumerator DeathSequenceWithMenu()
    {
        // Меняем спрайт героя на лежачий
        if (spriteRenderer != null && deadBodySprite != null)
            spriteRenderer.sprite = deadBodySprite;

        // Создаём духа в позиции героя
        GameObject spirit = null;
        if (spiritPrefab != null)
        {
            spirit = Instantiate(
                spiritPrefab,
                transform.position + spiritOffset,
                Quaternion.identity
            );
            var rb = spirit.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = true;
        }

        // Двигаем духа вверх 3 секунды
        if (spirit != null)
        {
            Vector3 start = spirit.transform.position;
            Vector3 end = start + new Vector3(0, 150f, 0);
            float duration = 3f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                spirit.transform.position = Vector3.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            spirit.transform.position = end;
        }
        
        // Показываем меню поражения
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null && gameManager.loserWindowByIncorrectEssence != null)
        {
            gameManager.loserWindowByIncorrectEssence.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
