using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    const float rayLength = 1.5f;

    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 0.1f; // moving speed of the player
    [Range(0.01f, 20.0f)] [SerializeField] private float jumpForce = 6.0f; // jump force of the player
    [Space(10)]
    public LayerMask groundLayer;

    private Rigidbody2D rigidBody;
    private Animator animator;

    private bool isFacingRight = true;
    private bool isWalking;

    private Vector2 startPosition;
    private int lives = 3;
    private int score = 0;

    private const int keysNumber = 3;
    private int keysFound = 0;

    private bool finishedLevel = false;

    private bool IsGrounded()
    {
        return Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value);
    }

	private void Jump() 
    {
        if (IsGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
        theScale.x = -theScale.x;
        transform.localScale = theScale;
	}

    private void Death()
    {
        lives -= 1;
        if (lives != 0)
        {
            this.transform.position = startPosition;
            Debug.Log("Lives: " + lives);
        }
        else
        {
            Debug.Log("Game Over");
        }
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Bonus"))
		{
            score += 1;
            Debug.Log("Score: " + score);

            other.gameObject.SetActive(false);
		}

        if (other.CompareTag("LevelEnd") && finishedLevel == false)
            if (keysFound == keysNumber)
		    {
                finishedLevel = true;
                Debug.Log("Level finished!");
		    }
            else
            {
                Debug.Log("Not enough keys");
            }

        if (other.CompareTag("Enemy"))
        {
            if (this.transform.position.y > other.transform.position.y)
            {
                score += 1;
                Debug.Log("Killed an enemy");
            }
            else
            {
                Death();
            }
        }

        if (other.CompareTag("Key"))
        {
            keysFound += 1;
            Debug.Log("Found key");

            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Heart"))
        {
            lives += 1;
            Debug.Log("Increased live to " + lives);

            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("FallLevel"))
        {
            Death();
        }

        if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        startPosition = this.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isWalking = false;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) 
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;
            if (!isFacingRight)
            {
                Flip();
            }
		}

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
			isWalking = true;
			if (isFacingRight)
			{
				Flip();
			}
		}

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

		// Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 0.1f, false);

		animator.SetBool("isGrounded", IsGrounded());
		animator.SetBool("isWalking", isWalking);
	}
}
