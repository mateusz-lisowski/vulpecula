using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f;
    [Range(0.01f, 20.0f)][SerializeField] private float moveRange = 1f;

    private float startPositionX = 0f;

    private bool isMovingRight = false;

    private void MoveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }
    private void MoveLeft()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

    private void Flip()
    {
        isMovingRight = !isMovingRight;
        Vector3 theScale = transform.localScale;
        theScale.x = -theScale.x;
        transform.localScale = theScale;
    }

    private void Awake()
    {
        startPositionX = this.transform.position.x;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingRight)
            if (this.transform.position.x < startPositionX + moveRange)
            {
                MoveRight();
            }
            else
            {
                Flip();
                isMovingRight = false;
            }
        else
            if (this.transform.position.x > startPositionX - moveRange)
        {
            MoveLeft();
        }
        else
        {
            Flip();
            isMovingRight = true;
        }
    }
}
