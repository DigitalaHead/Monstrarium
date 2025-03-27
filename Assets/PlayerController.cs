using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController movementController;

    [SerializeField]
    private EssenceManager essenceManager;

    // Start is called before the first frame update
    void Start()
    {
        movementController = GetComponent<MovementController>();
        essenceManager = FindFirstObjectByType<EssenceManager>(); // Находим объект EssenceManager в сцене
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        EssencePickup essencePickup = other.GetComponent<EssencePickup>();
        if (essencePickup != null)
        {
            Essence essence = essencePickup.GetEssence();
            if (essenceManager != null)
            {
                essenceManager.CollectEssence(essence);
                Debug.Log("Essence collected: " + essence.color);
            }
            else
            {
                Debug.LogError("EssenceManager is not assigned in the inspector!");
            }
            Destroy(other.gameObject);
        }
    }
}
