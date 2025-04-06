using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movementController;
    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    void Awake()
    {
        movementController = GetComponent<MovementController>();
        movementController.lastMovingDirection = "right";
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
            EssenceSpawner spawner = FindFirstObjectByType<EssenceSpawner>();
            if (spawner != null)
            {
                spawner.DestroyAndSpawnEssence(collision.gameObject); // Удаляем и спауним новую эссенцию
            }
            else
            {
                Debug.LogWarning("EssenceSpawner не найден!");
                Destroy(collision.gameObject); // Удаляем эссенцию, если спаунер не найден
            }
        }
    }
}
