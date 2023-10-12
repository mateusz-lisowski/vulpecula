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

    private BoxCollider2D groundCheck;
    private BoxCollider2D wallCheck;

    private bool isFacingRight = true;
    private bool isWalking = false;
    private bool isGrounded = false;
    private bool isFacingWall = false;

    private int score = 0;

    private void check_collisions()
    {
		isGrounded = groundCheck.IsTouchingLayers(groundLayer) && rigidBody.velocity.y <= 0;
		isFacingWall = wallCheck.IsTouchingLayers(groundLayer);
	}

	private void Jump() 
    {
        if (isGrounded)
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

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Bonus"))
		{
            score += 1;
            Debug.Log("Score: " + score);

            other.gameObject.SetActive(false);
		}
	}

	private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        groundCheck = transform.GetChild(0).GetComponent<BoxCollider2D>();
        wallCheck = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        check_collisions();
		isWalking = false;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) 
        {
            if (!isFacingWall)
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                isWalking = true;
            }
            if (!isFacingRight)
                Flip();
		}

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (!isFacingWall)
            {
                transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                isWalking = true;
            }
			if (isFacingRight)
				Flip();
		}

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

		animator.SetBool("isGrounded", isGrounded);
		animator.SetBool("isWalking", isWalking);
	}
}
