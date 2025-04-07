using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public bool canMoveLeft = false;
    public bool canMoveRight = false;
    public bool canMoveUp = false;
    public bool canMoveDown = false;

    public GameObject nodeLeft;
    public GameObject nodeRight;
    public GameObject nodeUp;
    public GameObject nodeDown;

    public bool isWarpRightNode = false;
    public bool isWarpLeftNode = false;

     public bool isGhostStartingNode = false;

    public GameManager gameManager;
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        RaycastHit2D[] hitsRight;
        hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right);

        for (int i = 0; i < hitsRight.Length; i++)
        {
            float distance = Mathf.Abs(hitsRight[i].point.x - transform.position.x);
            if (distance < 0.17f)
            {
                canMoveRight = true;
                nodeRight = hitsRight[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsLeft;
        hitsLeft = Physics2D.RaycastAll(transform.position, -Vector2.right);

        for (int i = 0; i < hitsLeft.Length; i++)
        {
            float distance = Mathf.Abs(hitsLeft[i].point.x - transform.position.x);
            if (distance < 0.17f)
            {
                canMoveLeft = true;
                nodeLeft = hitsLeft[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsUp;
        hitsUp = Physics2D.RaycastAll(transform.position, Vector2.up);

        for (int i = 0; i < hitsUp.Length; i++)
        {
            float distance = Mathf.Abs(hitsUp[i].point.y - transform.position.y);
            if (distance < 0.17f)
            {
                canMoveUp = true;
                nodeUp = hitsUp[i].collider.gameObject;
            }
        }

        RaycastHit2D[] hitsDown;
        hitsDown = Physics2D.RaycastAll(transform.position, -Vector2.up);

        for (int i = 0; i < hitsDown.Length; i++)
        {
            float distance = Mathf.Abs(hitsDown[i].point.y - transform.position.y);
            if (distance < 0.17f)
            {
                canMoveDown = true;
                nodeDown = hitsDown[i].collider.gameObject;
            }
        }

      if (isGhostStartingNode)
       {
            canMoveDown = true;
            nodeDown = gameManager.ghostNodeCenter;
       }  
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetNodeFromDirection(string direction)
    {
        if (direction == "left" && canMoveLeft)
        {
            return nodeLeft;
        }
        else if (direction == "right" && canMoveRight)
        {
            return nodeRight;
        }
        else if (direction == "up" && canMoveUp)
        {
            return nodeUp;
        }
        else if (direction == "down" && canMoveDown)
        {
            return nodeDown;
        }
        else
        {
            return null;
        }

    }
}
